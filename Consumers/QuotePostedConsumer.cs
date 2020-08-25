using MassTransit;
using RabbitMqConsistentHash.IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash.Consumers
{
    public class QuotePostedConsumer : IConsumer<IQuotePosted>
    {
        public Task Consume(ConsumeContext<IQuotePosted> context)
        {
            Console.WriteLine($"Consumed {context.Message.GetType().Name} for quote {context.Message.Id}.");
            return Task.CompletedTask;
        }
    }
}
