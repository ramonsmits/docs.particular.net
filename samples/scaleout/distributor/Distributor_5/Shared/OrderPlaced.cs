using NServiceBus;
using System;

public class OrderPlaced :
    IEvent
{
    public Guid OrderId { get; set; }
    public string WorkerName { get; set; }
}