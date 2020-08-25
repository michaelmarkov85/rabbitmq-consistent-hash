using System;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public class QuoteCreated : IQuoteCreated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime PickupDate { get; set; }
        public bool RequestedAllNetworkPartners { get; set; }
    }
}
