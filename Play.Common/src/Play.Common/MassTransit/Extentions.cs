using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extentions
    {
        public static IServiceCollection UseMassTransitWithRabbitmq(this IServiceCollection serviceProvider)
        {


            var builder = serviceProvider.BuildServiceProvider();
            var configuration = builder.GetService<IConfiguration>();
            var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
            var serverSettings = configuration.GetSection(nameof(ServerSetting)).Get<ServerSetting>();

            serviceProvider.AddMassTransit(config =>
            {
                config.AddConsumers(Assembly.GetEntryAssembly());

                config.UsingRabbitMq((context, configure) =>
                {
                    configure.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serverSettings.ServiceName, false));
                    configure.Host(rabbitMQSettings.Host);
                    configure.UseMessageRetry(x => { x.Interval(3, TimeSpan.FromSeconds(5)); });
                });
            });
            return serviceProvider;
        }
    }
}