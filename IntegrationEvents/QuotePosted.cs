using System;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public class QuotePosted
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTimeOffset PostingDate { get; set; }
    }
}
