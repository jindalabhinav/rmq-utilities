using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IRabbitMQPublisher
    {
        void QueuePublish(RabbitMQCustomMessage rabbitMQCustomMessage, RabbitMQConfiguration queueConfig);
        void QueuePublish(RabbitMQCustomMessage[] rabbitMQCustomMessage, RabbitMQConfiguration queueConfig);
    }
}
