using System.Text.Json;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Models
{
    public class DeadLetterMessage
    {
        public IDictionary<string, object> Headers { get; set; }
        public string QueueName { get; set; }
        public string Exchange { get; set; }
        public string ErrorAt { get; set; }
        public string ErrorName { get; set; }
        public string Error { get; set; }
        public ReadOnlyMemory<byte> Body { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
