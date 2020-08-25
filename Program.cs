using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMqConsistentHash.Consumers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading.Tasks;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using MassTransit.RabbitMqTransport.Topology.Entities;
using MassTransit.RabbitMqTransport.Topology.Specifications;
using MassTransit.Topology.Observers;
using MassTransit.Topology.Topologies;
using RabbitMQ.Client.Framing.Impl;
using RabbitMqConsistentHash.IntegrationEvents;

namespace RabbitMqConsistentHash
{
    class Program
    {
        public static AppConfig AppConfig { get; set; }

        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));

                    services.AddScoped<QuoteCreatedConsumer1>();
                    services.AddScoped<QuoteCreatedConsumer2>();

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddBus(ConfigureBus);
                    });

                    services.AddHostedService<MassTransitConsoleHostedService>();
                    services.AddHostedService<PublisherService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }

        static IBusControl ConfigureBus(IRegistrationContext<IServiceProvider> provider)
        {
            AppConfig = provider.GetRequiredService<IOptions<AppConfig>>().Value;

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                Action<IRabbitMqReceiveEndpointConfigurator> first =
                    ep =>
                    {
                        ep.ConfigureConsumeTopology = false;
                        ep.Bind(
                            "x-consistent-hash",
                            x =>
                            {
                                x.Durable = true;
                                x.ExchangeType = "x-consistent-hash";
                                x.RoutingKey = "10";
                                x.SetBindingArgument("hash-header", "quote-id");
                            });

                        ep.PrefetchCount = 1;

                        ep.Consumer<QuoteCreatedConsumer1>(provider.Container);
                    };
                cfg.ReceiveEndpoint("qq1", first);

                Action<IRabbitMqReceiveEndpointConfigurator> second =
                    ep =>
                    {
                        ep.ConfigureConsumeTopology = false;

                        ep.Bind(
                            "x-consistent-hash",
                            x =>
                            {
                                x.Durable = true;
                                x.ExchangeType = "x-consistent-hash";
                                x.RoutingKey = "10";
                                x.SetBindingArgument("hash-header", "quote-id");
                            });

                        ep.PrefetchCount = 1;

                        ep.Consumer<QuoteCreatedConsumer2>(provider.Container);
                    };

                cfg.ReceiveEndpoint("qq2", second);

                cfg.Host(AppConfig.Host, AppConfig.VirtualHost, h =>
                {
                    h.Username(AppConfig.Username);
                    h.Password(AppConfig.Password);
                });
            });
        }
    }
}
