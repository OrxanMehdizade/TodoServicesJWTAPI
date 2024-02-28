using TodoServicesJWTAPI.Models.DTOs.Auth;

namespace TodoServicesJWTAPI.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        void Publish<T>(T message, string queueName);
    }
}
