using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IRabbitMQSubscriber
    {
        RMQBasicGetMessage<T> BasicGet<T>(RabbitMQConfiguration subscriberQueueDetail) where T : class, new();
    }
}
