using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqConsistentHash.Consumers;
using RabbitMqConsistentHash.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

                    services.AddSingleton<Counter>();

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
                

                IRabbitMqHost host = cfg.Host(AppConfig.Host, AppConfig.VirtualHost, h =>
                {
                    h.Username(AppConfig.Username);
                    h.Password(AppConfig.Password);
                });

                // 1. Configuring consistent exchange

                string consistentExchangeName = "x-consistent-hash";
                DeclareConsistentExchange(host, consistentExchangeName);

                // 2. Configuring endpoints bindings to queues
                cfg.ReceiveEndpoint("qq1", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind(
                        "x-consistent-hash",
                        x =>
                        {
                            x.Durable = true;
                            x.ExchangeType = "x-consistent-hash";
                            x.RoutingKey = "10";
                        });

                    ep.PrefetchCount = 1;

                    ep.Consumer<QuoteCreatedConsumer1>(provider.Container);
                });

                cfg.ReceiveEndpoint("qq2", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind(
                        "x-consistent-hash",
                        x =>
                        {
                            x.Durable = true;
                            x.ExchangeType = "x-consistent-hash";
                            x.RoutingKey = "10";
                        });

                    ep.PrefetchCount = 1;

                    ep.Consumer<QuoteCreatedConsumer2>(provider.Container);
                });

                // 3. Binding message endpoints to consistent exchange
                cfg.ConnectBusObserver(new BindIncomingMessageExchangeAction(
                    host,
                    consistentExchangeName,
                    typeof(IQuoteCreated)
                ));
            });
        }

        private static void DeclareConsistentExchange(IRabbitMqHost host, string consistentExchangeName)
        {
            ConnectionFactory connectionFactory = host.Settings.GetConnectionFactory();

            using IConnection connection = connectionFactory.CreateConnection(host.Settings.Host);
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(consistentExchangeName,
                "x-consistent-hash",
                true,
                false,
                new Dictionary<string, object>()
                {
                    ["hash-header"] = "quote-id"
                });
        }
    }
}
