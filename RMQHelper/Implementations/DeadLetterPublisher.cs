using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Exceptions;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;
using System.Text;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class DeadLetterPublisher : IDeadLetterPublisher
    {
        private readonly ILogger logger;
        private readonly IDeadLetterMessageAssembler deadLetterMessageAssembler;

        public DeadLetterPublisher(ILogger logger, IDeadLetterMessageAssembler deadLetterMessageAssembler)
        {
            this.logger = logger;
            this.deadLetterMessageAssembler = deadLetterMessageAssembler;
        }

        public void Publish(IModel channel, RabbitMQConfiguration subscriberQueueDetail, MQMessageProcessException mpe)
        {
            DeadLetterMessage deadLetterMessage = deadLetterMessageAssembler.Assemble(subscriberQueueDetail, mpe);

            logger.Information($"DeadLetter Message Assembled is {deadLetterMessage}");

            DeclareDeadLetterQueue(channel, subscriberQueueDetail);

            var properties = CreateDeadLetterBasicProperties(channel, deadLetterMessage);

            var message = GetMessage(mpe.Message);

            logger.Information($"Beginning Basic Publish to DeadLetter Queue {subscriberQueueDetail.DeadLetterQueue}");

            BeginBasicPublish(channel, "", subscriberQueueDetail.DeadLetterQueue, false, properties, message);

            logger.Information($"Completed Publishing to DeadLetter Queue {subscriberQueueDetail.DeadLetterQueue}");
        }

        private void DeclareDeadLetterQueue(IModel channel, RabbitMQConfiguration subscriberQueueDetail)
        {
            try
            {
                logger.Information($"Declaring DeadLetter Queue: {subscriberQueueDetail.DeadLetterQueue}");

                channel.QueueDeclare(subscriberQueueDetail.DeadLetterQueue, subscriberQueueDetail.Durable, subscriberQueueDetail.Exclusive,
                                 subscriberQueueDetail.AutoDelete, subscriberQueueDetail.DeadLetterQueueArguments);
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"Declare DeadLetter Queue Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
        }

        private IBasicProperties CreateDeadLetterBasicProperties(IModel channel, DeadLetterMessage deadLetterMessage)
        {
            try
            {
                logger.Information("Creating DeadLetter Basic Properties");

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = deadLetterMessage.Headers;
                properties.Headers["source-queue"] = deadLetterMessage.QueueName;
                properties.Headers["error-at"] = deadLetterMessage.ErrorAt;
                properties.Headers["error-name"] = deadLetterMessage.ErrorName;
                properties.Headers["error"] = deadLetterMessage.Error;

                return properties;
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"CreateDeadLetterBasicProperties Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }

        }

        private byte[] GetMessage(string message)
        {
            logger.Information("Converting Message to Array of Bytes");

            return Encoding.UTF8.GetBytes(message);
        }

        private void BeginBasicPublish(IModel channel, string Exchange, string RoutingKey, bool Mandatory, IBasicProperties Properties, byte[] Message)
        {
            try
            {
                channel.BasicPublish(Exchange, RoutingKey, Mandatory, Properties, Message);
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"BeginBasicPublish Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
        }
    }
}
