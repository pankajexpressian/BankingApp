using RabbitMQ.Client;

namespace RabbitMQHelper
{
    public class MQHelper
    {
        private readonly string _hostName = "localhost";

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = _hostName, UserName="guest", Password="guest" };
            return factory.CreateConnection();
        }

        public IModel CreateModel(IConnection connection)
        {
            return connection.CreateModel();
        }

        public void DeclareExchange(IModel channel, string exchangeName, string exchangeType)
        {
            channel.ExchangeDeclare(exchangeName, exchangeType, durable: true, autoDelete: false);
        }

        public void DeclareQueue(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void BindQueue(IModel channel, string queueName, string exchangeName, string routingKey = "")
        {
            channel.QueueBind(queueName, exchangeName, routingKey);
        }

        public void PublishMessage(IModel channel, string exchangeName, string routingKey, byte[] message)
        {
            channel.BasicPublish(exchangeName, routingKey, null, message);
        }

    }
}
