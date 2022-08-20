using RabbitMQ.Client;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Models
{
    public class RMQBasicGetMessage<T>
    {
        /// <summary>
        /// Retrieves the Basic-class content header properties for this message.
        /// </summary>
        public IBasicProperties BasicProperties { get; set; }

        /// <summary>
        /// Retrieves the body of this message of type T.
        /// </summary>
        public T Body { get; set; }

        /// <summary>
        /// Retrieve the delivery tag for this message. See also <see cref="IModel.BasicAck"/>.
        /// </summary>
        public ulong DeliveryTag { get; set; }

        /// <summary>
        /// Retrieve the exchange this message was published to.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Retrieve the number of messages pending on the queue, excluding the message being delivered.
        /// </summary>
        /// <remarks>
        /// Note that this figure is indicative, not reliable, and can
        /// change arbitrarily as messages are added to the queue and removed by other clients.
        /// </remarks>
        public uint MessageCount { get; set; }

        /// <summary>
        /// Retrieve the redelivered flag for this message.
        /// </summary>
        public bool Redelivered { get; set; }

        /// <summary>
        /// Retrieve the routing key with which this message was published.
        /// </summary>
        public string RoutingKey { get; set; }
    }
}
