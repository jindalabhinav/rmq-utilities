namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces
{
    public interface IRabbitMQServerConfigFactory
    {
        RabbitMQ.Client.IConnectionFactory CreateConnectionFactory();
    }
}
