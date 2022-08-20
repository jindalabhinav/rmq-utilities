using System.Text;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper.Models
{
    public class RabbitMQServerConfiguration
    {
        public const string seperator = " => ";
        public string Hostname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Vhost { get; set; }
        public bool SslEnabled { get; set; } = true;
        public string SslProtocol { get; set; } = "Tls12";
        public bool AutomaticRecoveryEnabled { get; set; } = true;
        public ushort HeartBeatSeconds { get; set; } = 30;
        public double NetworkRecoveryTimeInterval { get; set; } = 10;


        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Hostname: {Hostname} {seperator}");
            builder.Append($"UserName : {UserName} {seperator}");
            builder.Append($"Password : {Password} {seperator}");
            builder.Append($"Port : {Port} {seperator}");
            builder.Append($"Vhost : {Vhost} {seperator}");
            builder.Append($"SslEnabled : {SslEnabled} {seperator}");
            builder.Append($"SslProtocol : {SslProtocol} {seperator}");
            builder.Append($"AutomaticRecoveryEnabled : {AutomaticRecoveryEnabled} {seperator}");
            builder.Append($"HeartBeatSeconds : {HeartBeatSeconds} {seperator}");
            builder.Append($"NetworkRecoveryTimeInterval : {NetworkRecoveryTimeInterval}");
            return builder.ToString();
        }
    }
}