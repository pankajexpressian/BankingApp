using Microsoft.AspNetCore.Mvc;
using RabbitMQHelper;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using AccountService.DTOs;
using AccountService.Services;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountServiceController : ControllerBase
    {
        private readonly AccounManagementService _accounManagementService;
        public AccountServiceController(AccounManagementService accounManagementService)
        {
            _accounManagementService = accounManagementService;
        }

        [HttpPost("create")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            var accountCreatedEvent = new
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.Balance
            };

            var status = _accounManagementService.CreateAccount(account);

            if (!status)
            {
                return BadRequest("Account Creation Failed");
            }

            return Ok("Account Created");
        }

        [HttpGet("get-statement")]
        public IActionResult GetAccountStatement(string accountNumber)
        {
            var accountStatement = _accounManagementService.GetAccountStatement(accountNumber);
            if (accountStatement == null)
            {
                return BadRequest("Account Statement Not Found");
            }
            return Ok(accountStatement);
        }

        [HttpPost("pdf-request")]
        public IActionResult RequestPdf([FromBody] PdfRequest pdfRequest)
        {
            var status = _accounManagementService.GeneratePDF(pdfRequest);
            if (!status)
            {
                return BadRequest("PDF Generation Request Failed");
            }

            return Ok("PDF Generation Request Placed");
        }
    }

    

    
}
