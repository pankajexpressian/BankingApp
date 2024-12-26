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
using System.Threading.Channels;

namespace PdfGenerationService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==============PdfGenerationService started...================");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("pdf_exchange", "direct");
            channel.QueueDeclare("pdf_queue", true, false, false);
            channel.QueueBind("pdf_queue", "pdf_exchange", "event-generate-pdf");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var pdfRequest = JsonSerializer.Deserialize<PdfRequest>(message);
                await GeneratePdf(pdfRequest, channel);
            };

            channel.BasicConsume("pdf_queue", true, consumer);
            Console.WriteLine("Waiting for PDF generation requests...");
            Console.ReadLine();
        }

        private static async Task GeneratePdf(PdfRequest pdfRequest, IModel channel)
        {
            var accountDetails = await GetAccountStatementAsync(pdfRequest.AccountNumber);
            
            Console.WriteLine($"Generated PDF for account {pdfRequest.AccountNumber} with transactions: {accountDetails}");            
            
            var jsonMessage = JsonSerializer.Serialize(pdfRequest);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            
            channel.BasicPublish("pdf_exchange", "event-pdf-generated", null, body);
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

            //Console.WriteLine("PDF generation complete.");

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
