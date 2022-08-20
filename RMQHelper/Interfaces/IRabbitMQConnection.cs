using RabbitMQ.Client;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IRabbitMQConnection
    {
        bool IsConnected();
        IConnection TryConnect();
        IModel CreateModel();
    }
}
