using Microsoft.Extensions.DependencyInjection;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Implementations;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Interfaces;
using SkyscraperThrottlingLambdaHelper.RMQHelper.Models;
using System.Text.Json;

namespace SkyscraperThrottlingLambdaHelper.RMQHelper
{
    public static class RabbitMQSubscriberExtension
    {
        public static IServiceCollection IncludeRabbitMQSubscriber(this IServiceCollection services, RabbitMQServerConfiguration rabbitMQServerConfiguration,
            RabbitMQConfiguration rabbitMQSubscriberConfiguration)
        {
            return services.AddRabbitMQSubscriberConfiguration(rabbitMQServerConfiguration, rabbitMQSubscriberConfiguration)
                .AddRabbitMQConnectionServices()
                .AddRabbitMQSubscriberDependentServices();
        }

        public static IServiceCollection AddRabbitMQSubscriberConfiguration(this IServiceCollection services, RabbitMQServerConfiguration rabbitMQServerConfiguration,
            RabbitMQConfiguration rabbitMQSubscriberConfiguration)
        {
            services.AddSingleton(rabbitMQServerConfiguration);
            services.AddSingleton(rabbitMQSubscriberConfiguration);
            return services;
        }

        public static RabbitMQConfiguration GenerateRMQSubscriberConfig(string configSection)
        {
            if (!string.IsNullOrEmpty(configSection))
                throw new ArgumentNullException("RMQSubscriberConfig");

            return JsonSerializer.Deserialize<RabbitMQConfiguration>(configSection);
        }

        public static RabbitMQServerConfiguration GenerateRMQServerConfig(string configSection)
        {
            if (!string.IsNullOrEmpty(configSection))
                throw new ArgumentNullException("RMQServerConfig");

            return JsonSerializer.Deserialize<RabbitMQServerConfiguration>(configSection);
        }

        public static IServiceCollection AddRabbitMQConnectionServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQServerConfigFactory, RabbitMQServerConfigFactory>();
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

            return services;
        }

        public static IServiceCollection AddRabbitMQSubscriberDependentServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQSubscriber, RabbitMQSubscriber>();
            services.AddSingleton<IDeadLetterMessageAssembler, DeadLetterMessageAssembler>();
            services.AddSingleton<IDeadLetterPublisher, DeadLetterPublisher>();

            return services;
        }
    }
}