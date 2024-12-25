using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NotificationService1
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("account_exchange", "topic");
            channel.QueueDeclare("notification_queue", true, false, false);
            channel.QueueBind("notification_queue", "account_exchange", "event_account_created");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Notification 1: Account created: {message}");
            };

            channel.BasicConsume("notification_queue", true, consumer);
            Console.ReadLine();
        }
    }
}
