using Grpc.Net.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PdfGenerationService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("pdf_exchange", "direct");
            channel.QueueDeclare("pdf_queue", true, false, false);
            channel.QueueBind("pdf_queue", "pdf_exchange", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var pdfRequest = JsonSerializer.Deserialize<PdfRequest>(message);
                await GeneratePdf(pdfRequest.AccountNumber);
            };

            channel.BasicConsume("pdf_queue", true, consumer);
            Console.WriteLine("Waiting for PDF generation requests...");
            Console.ReadLine();
        }

        private static async Task GeneratePdf(string accountNumber)
        {
            // Simulate calling gRPC service to get account statement.
            var accountDetails = await GetAccountStatementAsync(accountNumber);
            Console.WriteLine($"Generated PDF for account {accountNumber} with transactions: {accountDetails}");
            // Publish PDF generated event to RabbitMQ
        }

        private static async Task<AccountStatement> GetAccountStatementAsync(string accountNumber)
        {
            // Call gRPC method to get account statement
            return new AccountStatement { AccountNumber = accountNumber, Transactions = new List<string> { "Debit: 100", "Credit: 200" } };
        }
    }

    public class AccountStatement
    {
        public string AccountNumber { get; set; }
        public List<string> Transactions { get; set; }
    }

    public class PdfRequest
    {
        public string AccountNumber { get; set; }
    }
}
