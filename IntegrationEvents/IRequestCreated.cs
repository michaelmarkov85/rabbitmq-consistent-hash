using System;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public interface IRequestCreated
    {
        int Id { get; set; }
        int QuoteId { get; set; }
        Guid CapacityProvider { get; set; }
    }
}
