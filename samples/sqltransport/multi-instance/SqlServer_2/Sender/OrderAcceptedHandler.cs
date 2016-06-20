using System;
using System.Threading;
using Messages;
using NServiceBus;

public class OrderAcceptedHandler : IHandleMessages<ClientOrderAccepted>
{
    IBus bus;

    static int count;

    public OrderAcceptedHandler(IBus bus)
    {
        this.bus = bus;
    }

    public void Handle(ClientOrderAccepted message)
    {
        Console.WriteLine($"{DateTime.UtcNow} Received ClientOrderAccepted for ID {message.OrderId}");
        if (Interlocked.Increment(ref count) % 2 == 0) throw new InvalidOperationException("Failure!");
        Console.WriteLine("\tOK!");
    }
}