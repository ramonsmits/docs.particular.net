using System;
using NServiceBus;

public class ShipOrder :
    IMessage
{
    public Guid OrderId { get; set; }
    public bool Defer { get; set; }
}