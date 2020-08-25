using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMqConsistentHash.IntegrationEvents;

namespace RabbitMqConsistentHash
{
    public class PublisherService : IHostedService
    {
        readonly IBusControl _bus;

        public PublisherService(IBusControl bus)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.Publish(new QuoteCreated()
            {
                Id = 456
            }, pp => pp.Headers.Set("quote-id", 456));

            await _bus.Publish(new QuoteCreated()
            {
                Id = 555
            }, pp => pp.Headers.Set("quote-id", 555));

            await _bus.Publish(new QuoteCreated()
            {
                Id = 123
            }, pp => pp.Headers.Set("quote-id", 123));

            await _bus.Publish(new QuoteCreated()
            {
                Id = 1590
            }, pp => pp.Headers.Set("quote-id", 1590));

            //Console.WriteLine("Starting bus");
            //await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //Console.WriteLine("Stopping bus");
            //return _bus.StopAsync(cancellationToken);
        }
    }
}