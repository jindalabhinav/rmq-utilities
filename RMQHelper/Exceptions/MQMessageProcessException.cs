using System.Runtime.Serialization;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Exceptions
{
    [Serializable]
    public class MQMessageProcessException : Exception
    {
        public MQMessageProcessException()
        {
        }

        public MQMessageProcessException(string message) : base(message)
        {
        }

        public MQMessageProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MQMessageProcessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
