using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;
using System.Text;
using System.Text.Json;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class RabbitMQSubscriber : IRabbitMQSubscriber
    {
        private readonly ILogger logger;
        private readonly IRabbitMQConnection rabbitMQConnection;
        private readonly RabbitMQConfiguration rabbitMQSubscriberConfiguration;
        private readonly IDeadLetterPublisher deadLetterPublisher;
        IConnection connection;

        public RabbitMQSubscriber(ILogger logger,
            IRabbitMQConnection rabbitMQConnection,
            RabbitMQConfiguration rabbitMQSubscriberConfiguration,
            IDeadLetterPublisher deadLetterPublisher)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rabbitMQConnection = rabbitMQConnection ?? throw new ArgumentNullException(nameof(rabbitMQConnection));
            this.rabbitMQSubscriberConfiguration = rabbitMQSubscriberConfiguration ?? throw new ArgumentNullException(nameof(rabbitMQSubscriberConfiguration));
            this.deadLetterPublisher = deadLetterPublisher ?? throw new ArgumentNullException(nameof(deadLetterPublisher));
        }

        public RMQBasicGetMessage<T> BasicGet<T>(RabbitMQConfiguration subscriberQueueDetail) where T : class, new()
        {
            logger.Information($"Checking if Subscriber is Connected to Queue {subscriberQueueDetail.QueueName}");

            if (!rabbitMQConnection.IsConnected())
            {
                connection = rabbitMQConnection.TryConnect();
            }

            logger.Information("Creating RabbitMQ Consumer channel");

            var channel = rabbitMQConnection.CreateModel();

            logger.Information($"Declaring Queue Name {subscriberQueueDetail.QueueName}");

            DeclareQueue(channel, subscriberQueueDetail);

            logger.Information($"Beginning BasicGet from {subscriberQueueDetail.QueueName}");

            var basicGetResult = GetMessageBasic(channel, subscriberQueueDetail);

            var output = GenerateBasicGetMessage<T>(basicGetResult);

            return output;
        }

        private RMQBasicGetMessage<T> GenerateBasicGetMessage<T>(BasicGetResult basicGetResult)
        {
            var body = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(basicGetResult.Body.ToArray()));
            if (body == null)
                throw new ArgumentException($"BasicGet message failed to Deserialize with Model {nameof(T)}");

            return new RMQBasicGetMessage<T>()
            {
                BasicProperties = basicGetResult.BasicProperties,
                Body = body,
                DeliveryTag = basicGetResult.DeliveryTag,
                Exchange = basicGetResult.Exchange,
                MessageCount = basicGetResult.MessageCount,
                Redelivered = basicGetResult.Redelivered,
                RoutingKey = basicGetResult.RoutingKey
            };
        }

        private void DeclareQueue(IModel channel, RabbitMQConfiguration subscriberQueueDetail)
        {
            try
            {
                channel.QueueDeclare(queue: subscriberQueueDetail.QueueName,
                                 durable: subscriberQueueDetail.Durable,
                                 exclusive: subscriberQueueDetail.Exclusive,
                                 autoDelete: subscriberQueueDetail.AutoDelete,
                                 subscriberQueueDetail.QueueArguments);
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"Declare Queue Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
            catch (Exception ex)
            {
                logger.Error($"Exception occured in Declaring Queue:{ex}");
                throw;
            }
        }

        private BasicGetResult GetMessageBasic(IModel channel, RabbitMQConfiguration subscriberQueueDetail)
        {
            try
            {
                var message = channel.BasicGet(queue: subscriberQueueDetail.QueueName, autoAck: false);
                logger.Information($"BasicGet from {subscriberQueueDetail.QueueName} Succeded");
                return message;
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"GetMessageBasic Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
            catch (Exception ex)
            {
                logger.Error($"Exception Occured in GetMessageBasic with exception {ex}");
                throw;
            }
        }

        private void Disconnect()
        {
            logger.Information("Disconnecting Channel and Connection managed resources.");

            try
            {
                connection?.Dispose();

                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                logger.Error($"Exception Occured while Disconnecting {ex}");
            }
        }
    }
}
