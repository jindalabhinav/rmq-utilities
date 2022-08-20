using RabbitMQ.Client;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;
using System.Net.Security;
using System.Security.Authentication;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations
{
    public class RabbitMQServerConfigFactory : IRabbitMQServerConfigFactory
    {
        private readonly RabbitMQServerConfiguration rabbitMQServerConfiguration;

        public RabbitMQServerConfigFactory(RabbitMQServerConfiguration rabbitMQServerConfiguration)
        {
            this.rabbitMQServerConfiguration = rabbitMQServerConfiguration;
        }


        public RabbitMQ.Client.IConnectionFactory CreateConnectionFactory()
        {
            bool isValid = Enum.TryParse(rabbitMQServerConfiguration.SslProtocol, out SslProtocols sslProtocols);

            Validate();

            return new ConnectionFactory()
            {
                HostName = rabbitMQServerConfiguration.Hostname,
                UserName = rabbitMQServerConfiguration.UserName,
                Password = rabbitMQServerConfiguration.Password,
                VirtualHost = rabbitMQServerConfiguration.Vhost,
                Port = rabbitMQServerConfiguration.Port,
                RequestedHeartbeat = TimeSpan.FromSeconds(rabbitMQServerConfiguration.HeartBeatSeconds),
                AutomaticRecoveryEnabled = rabbitMQServerConfiguration.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(rabbitMQServerConfiguration.NetworkRecoveryTimeInterval),
                Ssl = new SslOption()
                {
                    Enabled = rabbitMQServerConfiguration.SslEnabled,
                    Version = sslProtocols,
                    AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNotAvailable
                }
            };
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(rabbitMQServerConfiguration.Hostname))
                throw new ArgumentNullException(nameof(rabbitMQServerConfiguration.Hostname));

            if (string.IsNullOrEmpty(rabbitMQServerConfiguration.UserName))
                throw new ArgumentNullException(nameof(rabbitMQServerConfiguration.UserName));

            if (string.IsNullOrEmpty(rabbitMQServerConfiguration.Password))
                throw new ArgumentNullException(nameof(rabbitMQServerConfiguration.Password));

            if (string.IsNullOrEmpty(rabbitMQServerConfiguration.Vhost))
                throw new ArgumentNullException(nameof(rabbitMQServerConfiguration.Vhost));

            if (rabbitMQServerConfiguration.Port == 0)
                throw new ArgumentNullException(nameof(rabbitMQServerConfiguration.Port));
        }
    }
}
