using Grpc.Net.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Myaccountservice;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Transactions;

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
            var grpcClient = new GrpcClient();

            // Simulate requesting account statement for account number "12345"
            var accountStatement = await grpcClient.GetAccountStatement("12345");

            Console.WriteLine($"Account Statement for {accountStatement.AccountNumber}:");
            foreach (var transaction in accountStatement.Transactions)
            {
                Console.WriteLine($"From: {transaction.FromAccount}, To: {transaction.ToAccount}, Amount: {transaction.Amount}");
            }

            Console.WriteLine("PDF generation complete.");

            return new AccountStatement
            {
                AccountNumber = accountStatement.AccountNumber,
                Transactions = accountStatement.Transactions.Select(t =>
                    $"{t.FromAccount} -> {t.ToAccount}: {t.TransactionType} {t.Amount} on {t.DateTime}").ToList()
            };

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
