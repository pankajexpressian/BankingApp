using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NotificationService2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("pdf_generated_exchange", "fanout");
            channel.ExchangeDeclare("account_exchange", "topic");

            channel.QueueDeclare("notification2_queue", true, false, false);
            channel.QueueBind("notification2_queue", "account_exchange", "event_account_created");
            channel.QueueBind("notification2_queue", "pdf_generated_exchange", "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Notification 2: Event received: {message}");
            };

            channel.BasicConsume("notification2_queue", true, consumer);
            Console.ReadLine();
        }
    }
}
