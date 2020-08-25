using System;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public interface IQuotePosted
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime PickupDate { get; set; }
        DateTimeOffset PostingDate { get; set; }
    }
}
