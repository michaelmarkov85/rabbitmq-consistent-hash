using System;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public class RequestCreated : IRequestCreated
    {
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public Guid CapacityProvider { get; set; }
    }
}
