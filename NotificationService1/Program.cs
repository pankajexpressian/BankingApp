using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace NotificationService1
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("==============Notification Service 1 started...================");

        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    //channel.ExchangeDeclare("account_exchange", "topic");
        //    //channel.QueueDeclare("notification1_queue", true, false, false);
        //    //channel.QueueBind("notification1_queue", "account_exchange", "event_account_created");

        //    channel.ExchangeDeclare("pdf_exchange", "direct");
        //    channel.QueueDeclare("pdf_queue", true, false, false);
        //    channel.QueueBind("pdf_queue", "pdf_exchange", "");

        //    var consumer = new EventingBasicConsumer(channel);
        //    consumer.Received += (sender, e) =>
        //    {
        //        var body = e.Body.ToArray();
        //        var message = Encoding.UTF8.GetString(body);
        //        Console.WriteLine($"Notification 1: PDF Generated: {message}");
        //    };

        //    channel.BasicConsume("notification1_queue", true, consumer);
        //    Console.ReadLine();
        //}

        static void Main(string[] args)
        {
            Console.WriteLine("==============Notification Service 1 started...================");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("pdf_exchange", "direct");
            channel.QueueDeclare("pdf_queue", true, false, false);
            channel.QueueBind("pdf_queue", "pdf_exchange", "event-pdf-generated");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Notification 1: PDF Generated: {message}");
            };

            channel.BasicConsume("pdf_queue", true, consumer);

            Console.ReadLine();
        }
    }
}
