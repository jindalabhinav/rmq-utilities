using System.Text;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Models
{
    public class RabbitMQConfiguration
    {
        public const string seperator = " => ";

        public string QueueName { get; set; }
        public bool AutoDelete { get; set; } = false;
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public string DeadLetterQueue { get; set; } = "FS_GLOBAL_DEAD_LETTER_Q";
        public IDictionary<string, object> QueueArguments { get; set; } = null;
        public IDictionary<string, object> DeadLetterQueueArguments { get; set; } = null;


        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"QueueName: {QueueName} {seperator}");
            builder.Append($"AutoDelete : {AutoDelete} {seperator}");
            builder.Append($"Durable : {Durable} {seperator}");
            builder.Append($"Exclusive : {Exclusive} {seperator}");
            builder.Append($"DeadLetterQueue : {DeadLetterQueue} {seperator}");
            return builder.ToString();
        }
    }
}