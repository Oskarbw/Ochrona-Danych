using api.Model;
using dotnet_docker.Model;
using dotnet_docker.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dotnet_docker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BankContext _db;
        private readonly Random _rand;
        public ExampleController(IConfiguration configuration, BankContext db)
        {
            _configuration = configuration;
            _db = db;
            _rand = new Random();
        }

        [HttpGet("hello")]
        public async Task<ActionResult<string>> Get()
        {
            return Ok("hello");
        }

        [HttpGet("env")]
        public async Task<ActionResult<string>> GetEnv()
        {
            var jwtKey = Environment.GetEnvironmentVariable("jwt_key");

            return Ok(jwtKey);
        }

        [HttpGet("time")]
        public async Task<ActionResult<string>> GetTime()
        {

            var timeSubtract = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var timeFunction = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var response = new
            {
                timeSubtract = timeSubtract,
                timeFunction = timeFunction
            };
            return Ok(response);
        }

        [HttpGet("random")]
        public async Task<ActionResult<int>> GetRandom()
        {
            var random = _rand.Next(2);

            return Ok(random);
        }
        

        [HttpGet("newton")]
        public async Task<ActionResult<int>> GetRandomNewton()
        {
            
            var numbers = Enumerable.Range(1, 8).OrderBy(x => _rand.Next()).Take(4);

            return Ok(numbers);
        }

        [HttpGet("entropy/{text}")]
        public async Task<ActionResult<int>> GetEntropy(string text)
        {

            var entropy = CryptoUtils.CalculateEntropy(text);

            return Ok(entropy);
        }

        [HttpGet("attempt/{username}")]
        public async Task<ActionResult<string>> GetAttempts(string username)
        {
            // tez mozna dodac wait sekundowy
            int currentTimeUnix = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var attempts = _db.Attempts.Where(x => x.Username == username && x.Time > currentTimeUnix - 10).ToList();

            if (!attempts.IsNullOrEmpty())
            {
                return Ok(attempts);
            }
            else
            {
                var newAttempt = new Attempt
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Ip = "123.123.123.123",
                    Time = currentTimeUnix

                };
                _db.Attempts.Add(newAttempt);
                _db.SaveChanges();

                return Ok("Nie bylo attemptow");
            }

            
        }

        [Authorize]
        [HttpGet("auth")]
        public async Task<ActionResult<string>> GetAuth()
        {

            return Ok("Udalos sie zautoryzowac");
        }

        

        [Authorize]
        [HttpGet("authuser")]
        public async Task<ActionResult<string>> GetAuthUser()
        {
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if(usernameFromToken == "mojUser")
            {
                return Ok("Userowe Udalos sie zautoryzowac");
            }
            return BadRequest("Nieudalo");
        }

        [HttpGet("token")]
        public async Task<ActionResult<string>> GetToken()
        {
            var token = CreateToken("mojUser");
            var response = new
            {
                token = token,
                username = "bomba",
                userId = "5432-b34"
            };
            return Ok(response);
        }

        
        [HttpGet("db")]
        public async Task<ActionResult<string>> GetDb()
        {
            var response = _db.Pumas.ToList();
            return Ok(response);
        }

        [HttpGet("ip")]
        public async Task<ActionResult<string>> GetIp()
        {


            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            var response = new
            {
                ip1 = ipAddress,
                ip2 = remoteIpAddress.ToString(),
                ip3 = remoteIpAddress.MapToIPv4().ToString()
            };
            return Ok(response);
        }

        [HttpGet("decimal/{number}")]
        public async Task<ActionResult<string>> GetDecimal(decimal number)
        {
            
            return Ok(number);
        }

        [HttpGet("double/{number}")]
        public async Task<ActionResult<string>> GetDouble(double number)
        {

            return Ok(number);
        }


        private string GetIPAddress()
        {

            return "";
        }

        private string CreateToken(string user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("jwt_key")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
            claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                audience: _configuration.GetSection("JwtSettings:Audience").Value!,
                issuer: _configuration.GetSection("JwtSettings:Issuer").Value!,
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }
    }
}
