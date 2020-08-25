﻿using System;
using MassTransit.Internals.Reflection;

namespace RabbitMqConsistentHash.IntegrationEvents
{
    public interface IQuoteCreated
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime PickupDate { get; set; }
        bool RequestedAllNetworkPartners { get; set; }
    }
}
