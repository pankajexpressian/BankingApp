using Microsoft.AspNetCore.Mvc;
using RabbitMQHelper;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountServiceController : ControllerBase
    {
        private readonly MQHelper _rabbitMqHelper;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public AccountServiceController(MQHelper rabbitMqHelper)
        {
            _rabbitMqHelper = rabbitMqHelper;
            _connection = _rabbitMqHelper.CreateConnection();
            _channel = _connection.CreateModel();
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

            // Publish event to RabbitMQ topic exchange
            var jsonMessage = JsonSerializer.Serialize(accountCreatedEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            _channel.BasicPublish("account_exchange", "event_account_created", null, body);

            return Ok("Account Created");
        }

        [HttpPost("pdf-request")]
        public IActionResult RequestPdf([FromBody] PdfRequest pdfRequest)
        {
            var jsonMessage = JsonSerializer.Serialize(pdfRequest);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            _channel.BasicPublish("pdf_exchange", "", null, body);

            return Ok("PDF Generation Request Placed");
        }
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
    }

    public class PdfRequest
    {
        public string AccountNumber { get; set; }
    }
}
