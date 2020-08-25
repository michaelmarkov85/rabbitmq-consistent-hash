using MassTransit;
using RabbitMqConsistentHash.IntegrationEvents;
using System;
using System.Threading.Tasks;

namespace RabbitMqConsistentHash.Consumers
{
    public class RequestCreatedConsumer : IConsumer<IRequestCreated>
    {
        public Task Consume(ConsumeContext<IRequestCreated> context)
        {
            Console.WriteLine($"Consumed {context.Message.GetType().Name} for quote {context.Message.QuoteId}.");
            return Task.CompletedTask;
        }
    }
}
