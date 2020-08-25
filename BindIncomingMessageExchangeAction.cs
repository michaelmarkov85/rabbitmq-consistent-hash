using MassTransit;
using MassTransit.RabbitMqTransport;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash
{
    public class BindIncomingMessageExchangeAction : IBusObserver
    {
        private readonly IRabbitMqHost _host;
        private readonly string _destinationExchangeName;
        private readonly Type[] _messageTypes;

        public BindIncomingMessageExchangeAction(IRabbitMqHost host, string destinationExchangeName, params Type[] messageTypes)
        {
            _host = host;
            _destinationExchangeName = destinationExchangeName;
            _messageTypes = messageTypes;
        }

        public async Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            await busReady;
            BindMessages();
        }

        public Task PostCreate(IBus bus) => Task.CompletedTask;
        public Task CreateFaulted(Exception exception) => Task.CompletedTask;
        public Task PreStart(IBus bus) => Task.CompletedTask;
        public Task StartFaulted(IBus bus, Exception exception) => Task.CompletedTask;
        public Task PreStop(IBus bus) => Task.CompletedTask;
        public Task PostStop(IBus bus) => Task.CompletedTask;
        public Task StopFaulted(IBus bus, Exception exception) => Task.CompletedTask;

        private void BindMessages()
        {
            ConnectionFactory connectionFactory = _host.Settings.GetConnectionFactory();

            using IConnection connection = connectionFactory.CreateConnection(_host.Settings.Host);
            using IModel channel = connection.CreateModel();

            var messageNameFormatter = new RabbitMqMessageNameFormatter();

            foreach (Type type in _messageTypes)
            {
                string source = messageNameFormatter.GetMessageName(type).Name;
                channel.ExchangeDeclare(source, ExchangeType.Fanout, true);
                channel.ExchangeBind(_destinationExchangeName, source, "");
            }
        }
    }
}