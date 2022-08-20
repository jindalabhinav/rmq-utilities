using SkyscraperThrottlingLambdaHelper.RMQHelper.Exceptions;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IDeadLetterMessageAssembler
    {
        DeadLetterMessage Assemble(RabbitMQConfiguration subscriberQueueDetail, MQMessageProcessException mpe);
    }
}
