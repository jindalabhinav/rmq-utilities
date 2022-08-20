namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Models
{
    public class RabbitMQCustomMessage
    {
        public RabbitMQCustomMessage()
        {

        }

        RabbitMQCustomMessage(string message, int priority = 0, Dictionary<string, object> headers = null, string correlationId = "")
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Priority = Convert.ToByte(priority);
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            if (!string.IsNullOrEmpty(correlationId))
                CorrelationId = correlationId;
        }

        public string Message { get; set; }
        public byte Priority { get; set; }
        public Dictionary<string, object> Headers { get; set; } = null;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }
}
