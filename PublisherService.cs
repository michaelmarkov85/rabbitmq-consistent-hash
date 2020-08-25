using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMqConsistentHash.IntegrationEvents;

namespace RabbitMqConsistentHash
{
    public class PublisherService : IHostedService
    {
        readonly IBusControl _bus;
        private readonly Counter _counter;
        private readonly ILogger<PublisherService> _logger;
        private readonly int[] _quoteIds = { 123, 234, 345, 456, 567, 678, 789, 890 };


        public PublisherService(IBusControl bus, Counter counter, ILogger<PublisherService> logger)
        {
            _bus = bus;
            _counter = counter;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                var quoteId = _quoteIds[new Random().Next(0, _quoteIds.Length - 1)];
                tasks.Add(_bus.Publish(
                    new QuoteCreated { Id = quoteId },
                    ctx => ctx.Headers.Set("quote-id", ctx.Message.Id),
                    CancellationToken.None));
            }

            await Task.WhenAll(tasks);

            await Task.Delay(5000, cancellationToken);

            _logger.LogInformation("Publishing finished");

            _logger.LogInformation($"Received {_counter.Result.Count} events.");

            foreach (var gr in _counter.Result.GroupBy(k => k.Key).OrderBy(x => x.Key))
            {
                var counters = gr
                    .GroupBy(x => x.Value)
                    .ToDictionary(k => k.Key, v => v.Count())
                    .Select(x => $"{x.Key}={x.Value}");

                _logger.LogInformation($"-load #{gr.Key}: {string.Join(" | ", counters)}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping bus");
            return _bus.StopAsync(cancellationToken);
        }
    }
}