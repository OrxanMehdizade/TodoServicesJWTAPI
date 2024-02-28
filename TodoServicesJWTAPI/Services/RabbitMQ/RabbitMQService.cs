using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TodoServicesJWTAPI.Services.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Publish<T>(T message, string queueName)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var msjBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(msjBody);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}
