using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using SkyscraperThrottlingLambdaHelper.RMQHelper;

namespace SkyscraperThrottlingLambdaHelper
{
    public class DIResolver
    {
        public static IServiceProvider _serviceProvider;
        public static IConfiguration _config;

        public void ResolveServices()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("/tmp/appsettings.json", optional: false, reloadOnChange: true)
               .AddSecretFile();
            _config = builder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddScoped(e => SetLogger())
                .AddRabbitMQSubscriberConfiguration(RabbitMQSubscriberExtension.GenerateRMQServerConfig(_config["rabbitMQServer"]), RabbitMQSubscriberExtension.GenerateRMQSubscriberConfig(_config["rabbitMQServer"]))
                .AddRabbitMQConnectionServices()
                .AddRabbitMQSubscriberDependentServices()
                .AddRabbitMQPublisherDependentServices();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public ILogger SetLogger()
        {
            LogContext.Reset();
            string env = Environment.GetEnvironmentVariable("LAMBDA_EXECUTION_ENVIRONMENT");

            if (!string.IsNullOrEmpty(env) && env.Equals("Local"))
            {
                // Log to RollingFile
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_config)
                    .WriteTo.RollingFile(pathFormat: _config["Serilog:WriteTo:1:PathFormat"], outputTemplate: _config["Serilog:WriteTo:1:OutputTemplate"])
                    .Enrich.FromLogContext()
                    .CreateLogger();
            }
            else
            {
                // Log to Logentries 
                Log.Logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(_config)
                     .WriteTo.Logentries(_config["Serilog:WriteTo:0:LogentriesToken"], outputTemplate: _config["Serilog:WriteTo:0:OutputTemplate"])
                     .Enrich.FromLogContext()
                     .CreateLogger();
            }
            return Log.Logger;
        }
    }
}
