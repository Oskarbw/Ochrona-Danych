using api.Model;
using BCrypt.Net;
using dotnet_docker.Model;
using dotnet_docker.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace dotnet_docker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BankContext _db;
        private readonly CryptoUtils _crypto;
        

        public UserController(IConfiguration configuration, BankContext db)
        {
            _configuration = configuration;
            _db = db;
            _crypto = new CryptoUtils();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<string>> GetUserFromUsername(string username)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            int currentTimeUnix = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var attempts = _db.Attempts.Where(x => x.IsLogin == false && x.Ip == ip
            && x.Time > currentTimeUnix - 10).ToList();

            if (!attempts.IsNullOrEmpty())
            {
                return StatusCode(429);
            }
            else
            {
                var numberOfTries = _db.Attempts.Where(x => x.IsLogin == false && x.Ip == ip
                && x.Time > currentTimeUnix - 900).ToList();
                if (numberOfTries != null && numberOfTries.Count >= 10) { return StatusCode(429); }

                var newAttempt = new Attempt
                {
                    Id = Guid.NewGuid(),
                    Username = "",
                    Ip = ip,
                    Time = currentTimeUnix,
                    IsLogin = false
                };
                _db.Attempts.Add(newAttempt);
                _db.SaveChanges();
            }







            var user = _db.Users.FirstOrDefault(x => x.Username == username);
            if (user == null)
            {
                return BadRequest();
            }

            var randomNumber = _crypto.randomNumber(1, 11);

            var passwordVariant = _db.Passwords.FirstOrDefault(x => x.Username == username && x.Variant == randomNumber);
            if (passwordVariant != null)
            {
                var response = new
                {
                    variant = passwordVariant.Variant,
                    pattern = passwordVariant.Pattern
                };
                return Ok(response);
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            int currentTimeUnix = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var attempts = _db.Attempts.Where(x => (x.Username == loginDto.Username || x.Ip == ip)
            && x.Time > currentTimeUnix - 10 && x.IsLogin == true).ToList();

            if (!attempts.IsNullOrEmpty())
            {
                return StatusCode(429);
            }
            else
            {
                var numberOfTries = _db.Attempts.Where(x => (x.Username == loginDto.Username || x.Ip == ip)
                && x.Time > currentTimeUnix - 900 && x.IsLogin == true).ToList();
                if (numberOfTries != null && numberOfTries.Count >= 10) { return StatusCode(429); }

                var newAttempt = new Attempt
                {
                    Id = Guid.NewGuid(),
                    Username = loginDto.Username,
                    Ip = ip,
                    Time = currentTimeUnix,
                    IsLogin = true
                };
                _db.Attempts.Add(newAttempt);
                _db.SaveChanges();
            }

            



            var user = _db.Users.FirstOrDefault(x =>x.Username == loginDto.Username);
            if (user == null) { return BadRequest(); }


            var passwordEntity = _db.Passwords
                .FirstOrDefault(x => x.Username == loginDto.Username && x.Variant == loginDto.Variant);
            if (passwordEntity == null) { return BadRequest(); }

            var isVerified = _crypto.verifyPassword(loginDto.PasswordFragment, passwordEntity.Hash);
            if (!isVerified) { return BadRequest(); }

            var token = CreateToken(loginDto.Username);

            var response = new
            {
                token = token,
                username = user.Username,
                accountNumber = user.AccountNumber,
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("balance")]
        public async Task<ActionResult<string>> GetBalance()
        {
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if (usernameFromToken == null) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(x => x.Username == usernameFromToken);
            if (user == null) { return BadRequest(); }

            decimal balance = user.Balance;
            var response = new
            {
                balance = balance
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("sensitive")]
        public async Task<ActionResult<string>> GetSensitive()
        {
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if (usernameFromToken == null) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(x => x.Username == usernameFromToken);
            if (user == null) { return BadRequest(); }

            var cardNumberEncrypted = user.CardNumber;
            var documentNumberEncrypted = user.DocumentNumber;

            var cardNumberDecrypted = CryptoUtils.Decryption(cardNumberEncrypted);
            var documentNumberDecrypted = CryptoUtils.Decryption(documentNumberEncrypted);

            var response = new
            {
                cardNumber = cardNumberDecrypted,
                documentNumber = documentNumberDecrypted,
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<ActionResult<string>> ChangePassword([FromBody] PasswordChangeDto passwordChangeDto)
        {   
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if (usernameFromToken == null) { return StatusCode(410); }

            var user = _db.Users.FirstOrDefault(x => x.Username == usernameFromToken);
            if (user == null) { return StatusCode(411); }

            if (passwordChangeDto.Password.Length < 8
                || passwordChangeDto.Password.Length > 20) { return BadRequest("Password must contain between 8 and 20 characters"); }
            if (!doesPasswordSatisfyContidions(passwordChangeDto.Password)) { return BadRequest("Password must have at least: 1 lowercase, 1 uppercase and 1 number"); }
            var entropy = CryptoUtils.CalculateEntropy(passwordChangeDto.Password);
            if (entropy < 2.5) {  return BadRequest("Password's entropy is too low: " + entropy.ToString()); }

            

            var newPassword = passwordChangeDto.Password;

            var currentPasswords = _db.Passwords.Where(x => x.Username == usernameFromToken).ToList();
            if (currentPasswords == null) { return BadRequest(); }

            _db.Passwords.RemoveRange(currentPasswords);

            var newPasswordEntities = _crypto.createTenPasswordEntities(newPassword, usernameFromToken);
            
            _db.Passwords.AddRange(newPasswordEntities);
            _db.SaveChanges();

            return Ok();
        }
        

        private string CreateToken(string username)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("jwt_key")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
            claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                audience: _configuration.GetSection("JwtSettings:Audience").Value!,
                issuer: _configuration.GetSection("JwtSettings:Issuer").Value!,
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

        private bool doesPasswordSatisfyContidions(string password)
        {
            var regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,20}$";
            return Regex.IsMatch(password, regex);
        }

    }
}
