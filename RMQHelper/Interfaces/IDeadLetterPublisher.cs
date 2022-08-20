using RabbitMQ.Client;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Exceptions;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IDeadLetterPublisher
    {
        void Publish(IModel channel, RabbitMQConfiguration subscriberQueueDetail, MQMessageProcessException mpe);
    }
}
