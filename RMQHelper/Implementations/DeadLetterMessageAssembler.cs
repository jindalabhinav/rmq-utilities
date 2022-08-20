using SkyscraperThrottlingLambdaHelper.RMQHelper.Exceptions;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class DeadLetterMessageAssembler : IDeadLetterMessageAssembler
    {
        public DeadLetterMessage Assemble(RabbitMQConfiguration subscriberQueueDetail, MQMessageProcessException mpe)
        {
            return new DeadLetterMessage()
            {
                Headers = new Dictionary<string, object>(),
                QueueName = subscriberQueueDetail?.QueueName ?? string.Empty,
                ErrorAt = DateTime.Now.ToString("yyyy-MMM-dd'T'HH:mm:ss'+0530'"),
                ErrorName = mpe?.GetType()?.FullName ?? string.Empty,
                Error = mpe?.ToString() ?? string.Empty,
            };
        }
    }
}
