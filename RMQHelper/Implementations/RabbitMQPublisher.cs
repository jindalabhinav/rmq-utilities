using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;
using System.Text;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly ILogger logger;
        private readonly IRabbitMQConnection rabbitMQConnection;

        public RabbitMQPublisher(ILogger logger, IRabbitMQConnection rabbitMQConnection)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rabbitMQConnection = rabbitMQConnection ?? throw new ArgumentNullException(nameof(rabbitMQConnection));
        }


        public void QueuePublish(RabbitMQCustomMessage rabbitMQCustomMessage, RabbitMQConfiguration queueConfig)
        {
            logger.Information("Beginning Queue Publish");

            Connect();

            using (var channel = rabbitMQConnection.CreateModel())
            {
                channel.ConfirmSelect();
                channel.BasicNacks += Channel_BasicNacks;
                PublishMessage(rabbitMQCustomMessage, channel, queueConfig);
                var isOK = channel.WaitForConfirms();
                if (!isOK)
                    logger.Error($"Messages wasn't pushed to Queue: '{queueConfig.QueueName}' successfully");
            }
        }

        public void QueuePublish(RabbitMQCustomMessage[] rabbitMQCustomMessages, RabbitMQConfiguration queueConfig)
        {
            logger.Information("Beginning Queue Publish");

            Connect();

            using (var channel = rabbitMQConnection.CreateModel())
            {
                channel.ConfirmSelect();
                channel.BasicNacks += Channel_BasicNacks;
                foreach (var message in rabbitMQCustomMessages)
                    PublishMessage(message, channel, queueConfig);
                var isOK = channel.WaitForConfirms();
                if (!isOK)
                    logger.Error($"Not all the messages were pushed to Queue: '{queueConfig.QueueName}' successfully");
            }
        }

        private void Channel_BasicNacks(object? sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            logger.Error($"Message with DeliveryTag: {e.DeliveryTag} didn't reach the RabbitMQ server");
        }

        private void PublishMessage(RabbitMQCustomMessage rabbitMQCustomMessage, IModel channel, RabbitMQConfiguration queueConfig)
        {
            if (string.IsNullOrEmpty(queueConfig.QueueName) || string.IsNullOrEmpty(rabbitMQCustomMessage.Message))
            {
                logger.Information("Queue-Name/Message is Null or Empty, Hence Aborting Publishing into RMQ Queue");

                throw new Exception("Queue-Name/Message cannot be Null or Empty");
            }

            QueueDeclare(channel, queueConfig);

            var properties = CreateBasicProperties(channel, rabbitMQCustomMessage);

            var message = GetMessageInBytes(rabbitMQCustomMessage.Message);

            logger.Information($"Beginning Basic Publish to RabbitMQ Queue {queueConfig.QueueName}");

            BeginBasicPublish(channel, "", queueConfig.QueueName, false, properties, message);

            logger.Information($"Succesfully published to RabbitMQ Queue {queueConfig.QueueName}");
        }

        private void Connect()
        {
            logger.Information($"Checking if Publisher is Connected to RabbitMQ Cluster");

            if (!rabbitMQConnection.IsConnected())
            {
                logger.Information($"No Active Connections found, Hence Opening New RabbitMQ Connection");

                rabbitMQConnection.TryConnect();
            }
        }

        private void QueueDeclare(IModel channel, RabbitMQConfiguration queueConfig)
        {
            try
            {
                logger.Information($"Declaring Queue: {queueConfig.QueueName}");

                channel.QueueDeclare(queueConfig.QueueName, queueConfig.Durable, queueConfig.Exclusive,
                    queueConfig.AutoDelete, queueConfig.QueueArguments);
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"Declare Queue Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
        }

        private IBasicProperties CreateBasicProperties(IModel channel, RabbitMQCustomMessage customMessage)
        {
            try
            {
                logger.Information("Creating Basic Properties");

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = customMessage.Headers;
                properties.CorrelationId = customMessage.CorrelationId;
                properties.Priority = customMessage.Priority;
                return properties;
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"CreateBasicProperties Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }

        }

        private byte[] GetMessageInBytes(string message)
        {
            logger.Information("Converting Message to Array of Bytes");

            return Encoding.UTF8.GetBytes(message);
        }

        private void BeginBasicPublish(IModel channel, string exchange, string routingKey, bool mandatory, IBasicProperties properties, byte[] message)
        {
            try
            {
                channel.BasicPublish(exchange, routingKey, mandatory, properties, message);
            }
            catch (OperationInterruptedException oie)
            {
                logger.Error($"BeginBasicPublish Operation Interrupted with Initiator {oie.ShutdownReason.Initiator}, Cause:{oie.ShutdownReason.Cause}, Exception: {oie}");
                throw;
            }
        }
    }
}
