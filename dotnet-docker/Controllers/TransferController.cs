using api.Model;
using dotnet_docker.Model;
using dotnet_docker.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
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
    public class TransferController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BankContext _db;
        private readonly CryptoUtils _crypto;


        public TransferController(IConfiguration configuration, BankContext db)
        {
            _configuration = configuration;
            _db = db;
            _crypto = new CryptoUtils();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<string>> GetTransfers()
        {
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if (usernameFromToken == null) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(x => x.Username == usernameFromToken);
            if (user == null) { return BadRequest(); }

            var transfers = _db.Transfers
                .Where(x => x.SenderUsername == usernameFromToken
                || x.ReceiverUsername == usernameFromToken).ToList();
            if (transfers == null || transfers.Count == 0) {  return BadRequest(); }

            var sortedTransfers = transfers.OrderBy(x => x.Time).ToList();
            sortedTransfers.Reverse();
            var mappedTransfers = new List<TransferGetDto>();

            for (int i = 0; i < sortedTransfers.Count; i++)
            {   
                if (sortedTransfers[i].SenderUsername == usernameFromToken)
                {
                    var receiver = _db.Users.FirstOrDefault(x => x.Username == sortedTransfers[i].ReceiverUsername);
                    var mappedTransfer = new TransferGetDto
                    {
                        Title = sortedTransfers[i].Title,
                        ReceiverName = sortedTransfers[i].ReceiverName,
                        ReceiverAdress = sortedTransfers[i].ReceiverAdress,
                        SenderName = "You",
                        ReceiverAccountNumber = receiver.AccountNumber,
                        Amount = sortedTransfers[i].Amount,
                        Time = sortedTransfers[i].Time
                    };
                    mappedTransfers.Add(mappedTransfer);
                } else
                {
                    var senderAccountAddress = _db.Users.FirstOrDefault(x => x.Username == sortedTransfers[i].SenderUsername);

                    var mappedTransfer = new TransferGetDto
                    {
                        Title = sortedTransfers[i].Title,
                        ReceiverName = "You",
                        ReceiverAdress = "Your address",
                        SenderName = senderAccountAddress?.AccountNumber ?? "unknown",
                        ReceiverAccountNumber = user.AccountNumber,
                        Amount = sortedTransfers[i].Amount,
                        Time = sortedTransfers[i].Time
                    };
                    mappedTransfers.Add(mappedTransfer);
                }
                
            }

            var response = new
            {
                transfers = mappedTransfers
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> PostTransfer(TransferDto transferDto)
        {
            var usernameFromToken = User.FindFirst(ClaimTypes.Name)?.Value;
            if (usernameFromToken == null) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(x => x.Username == usernameFromToken);
            if (user == null) { return BadRequest(); }

            if (transferDto.Amount > 1000000m || transferDto.Amount < 0m ) { return BadRequest(); }

            var floorAmount = FloorTo2DecimalPlaces(transferDto.Amount);

            if (floorAmount <= 0) { return BadRequest(); }
            var validatedAmount = floorAmount;

            var isBalanceSufficient = user.Balance >= validatedAmount;
            if (!isBalanceSufficient) { return BadRequest(); }

            var receiver = _db.Users.FirstOrDefault(x => x.AccountNumber == transferDto.ReceiverAccountNumber);
            if (receiver == null) { return BadRequest(); }

            var receiverUsername = receiver.Username;

            user.Balance = user.Balance - validatedAmount;
            receiver.Balance = receiver.Balance + validatedAmount;

            var transfer = new Transfer
            {
                Id = Guid.NewGuid(),
                Title = transferDto.Title,
                ReceiverName = transferDto.ReceiverName,
                ReceiverAdress = transferDto.ReceiverAdress,
                SenderUsername = usernameFromToken,
                ReceiverUsername = receiverUsername,
                Amount = validatedAmount,
                Time = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            _db.Transfers.Add(transfer);
            _db.SaveChanges();

            return Ok();
        }

        private decimal FloorTo2DecimalPlaces(decimal number)
        {
            decimal scale = 100m; 
            decimal scaledNumber = number * scale;
            decimal roundedDown = Math.Floor(scaledNumber);
            return roundedDown / scale;
        }
    }
}
