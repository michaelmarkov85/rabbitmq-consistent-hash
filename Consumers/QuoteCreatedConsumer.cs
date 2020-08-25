using MassTransit;
using RabbitMqConsistentHash.IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash.Consumers
{

    public class QuoteCreatedConsumer1 : QuoteCreatedConsumer
    {
        public QuoteCreatedConsumer1() : base(1)
        {
        }
    }

    public class QuoteCreatedConsumer2 : QuoteCreatedConsumer
    {
        public QuoteCreatedConsumer2() : base(2)
        {
        }
    }

    public class QuoteCreatedConsumer : IConsumer<IQuoteCreated>
    {
        private readonly int _consumerNumber;

        public QuoteCreatedConsumer(int consumerNumber)
        {
            _consumerNumber = consumerNumber;
        }

        public Task Consume(ConsumeContext<IQuoteCreated> context)
        {
            Console.WriteLine($"Consumed {context.Message.GetType().Name} for quote {context.Message.Id}. Consumer {_consumerNumber}.");
            return Task.CompletedTask;
        }
    }
}
