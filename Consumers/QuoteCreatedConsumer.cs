using MassTransit;
using RabbitMqConsistentHash.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash.Consumers
{

    public class QuoteCreatedConsumer1 : QuoteCreatedConsumer
    {
        public QuoteCreatedConsumer1(Counter counter) : base(1, counter)
        {
        }
    }

    public class QuoteCreatedConsumer2 : QuoteCreatedConsumer
    {
        public QuoteCreatedConsumer2(Counter counter) : base(2, counter)
        {
        }
    }

    public class QuoteCreatedConsumer : IConsumer<IQuoteCreated>
    {
        private readonly int _consumerNumber;
        private readonly Counter _counter;

        public QuoteCreatedConsumer(int consumerNumber, Counter counter)
        {
            _consumerNumber = consumerNumber;
            _counter = counter;
        }

        public Task Consume(ConsumeContext<IQuoteCreated> context)
        {
            _counter.Result.Add(new KeyValuePair<int, int>(context.Message.Id, _consumerNumber));
            return Task.CompletedTask;
        }
    }
}
