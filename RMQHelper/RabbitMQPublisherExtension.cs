using Microsoft.Extensions.DependencyInjection;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper
{
    public static class RabbitMQPublisherExtension
    {
        public static IServiceCollection IncludeRabbitMQPublisher(this IServiceCollection services, RabbitMQServerConfiguration rabbitMQServerConfiguration)
        {
            return services.AddRabbitMQPublisherConfiguration(rabbitMQServerConfiguration)
                .AddRabbitMQBrokerConnectionServices()
                .AddRabbitMQPublisherDependentServices();
        }

        public static IServiceCollection AddRabbitMQPublisherConfiguration(this IServiceCollection services, RabbitMQServerConfiguration rabbitMQServerConfiguration)
        {
            services.AddSingleton(rabbitMQServerConfiguration);
            return services;
        }

        public static IServiceCollection AddRabbitMQBrokerConnectionServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQServerConfigFactory, RabbitMQServerConfigFactory>();
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

            return services;
        }

        public static IServiceCollection AddRabbitMQPublisherDependentServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQServerConfigFactory, RabbitMQServerConfigFactory>();
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

            return services;
        }
    }
}
