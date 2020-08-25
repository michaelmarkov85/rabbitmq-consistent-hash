using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash
{
    public class MassTransitConsoleHostedService : IHostedService
    {
        readonly IBusControl _bus;
        private readonly ILogger<MassTransitConsoleHostedService> _logger;

        public MassTransitConsoleHostedService(IBusControl bus, ILogger<MassTransitConsoleHostedService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus");
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bus");
            return _bus.StopAsync(cancellationToken);
        }
    }
}