using RabbitMQHelper;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using AccountService.DTOs;
using System.Threading.Channels;

namespace AccountService.Services
{
    public class AccounManagementService
    {

        private readonly MQHelper _rabbitMqHelper;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public AccounManagementService(MQHelper rabbitMqHelper)
        {
            _rabbitMqHelper = rabbitMqHelper;
            _connection = _rabbitMqHelper.CreateConnection();
            _channel = _connection.CreateModel();            
        }

        public bool CreateAccount(Account account)
        {
            var accountCreatedEvent = new
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.Balance
            };

            //_channel.ExchangeDeclare("account_exchange", "topic");
            //_channel.QueueDeclare("notification2_queue", true, false, false);
            //_channel.QueueBind("notification2_queue", "account_exchange", "event_account_created");

            // Publish event to RabbitMQ topic exchange
            var jsonMessage = JsonSerializer.Serialize(accountCreatedEvent);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            _channel.BasicPublish("account_exchange", "event_account_created", null, body);

            return true;
        }

        public bool GeneratePDF(PdfRequest pdfRequest)
        {
            var jsonMessage = JsonSerializer.Serialize(pdfRequest);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            _channel.BasicPublish("pdf_exchange", "event-generate-pdf", null, body);

            return true;
        }

        public object GetAccountStatement(string accountNumber)
        {
            throw new NotImplementedException();
        }
    }
}
