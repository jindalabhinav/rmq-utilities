using RabbitMQ.Client;
using Serilog;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        IConnection connection;
        bool disposed;
        private readonly ILogger logger;
        private readonly IRabbitMQServerConfigFactory rabbitMQServerConfigFactory;
        private IConnectionFactory connectionFactory;

        public RabbitMQConnection(ILogger logger, IRabbitMQServerConfigFactory rabbitMQServerConfigFactory)
        {
            this.logger = logger;
            this.rabbitMQServerConfigFactory = rabbitMQServerConfigFactory;
            connectionFactory = rabbitMQServerConfigFactory.CreateConnectionFactory();
        }


        public bool IsConnected()
        {
            if (connection != null && connection.IsOpen)
                return true;

            return false;
        }

        public IModel CreateModel()
        {
            if (!IsConnected())
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

            return connection.CreateModel();
        }

        public IConnection TryConnect()
        {
            logger.Information($"Creating RabbitMQ Connection.");


            if (connectionFactory == null)
                connectionFactory = rabbitMQServerConfigFactory.CreateConnectionFactory();

            connection = connectionFactory?.CreateConnection();

            logger.Information($"The connection acquired is {connection}");


            if (IsConnected())
                logger.Information($"RabbitMQ Client acquired a persistent connection to {connection.Endpoint.HostName}");

            else
            {
                logger.Fatal("FATAL ERROR: RabbitMQ Connection could not be created and opened");

                throw new Exception($"RabbitMQ Connection could not be created and opened with Connection Factory: {connectionFactory}");
            }

            return connection;
        }

        private void Disconnect()
        {
            logger.Information("Disconnecting Channel and Connection managed resources.");

            try
            {
                connection.Dispose();

                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                logger.Error($"Exception Occured while Disconnecting {ex}");
            }
        }

        public void Dispose()
        {
            if (disposed) return;

            disposed = true;

            try
            {
                connection.Dispose();
            }
            catch (IOException ex)
            {
                logger.Fatal(ex.ToString());
            }
        }
    }
}
