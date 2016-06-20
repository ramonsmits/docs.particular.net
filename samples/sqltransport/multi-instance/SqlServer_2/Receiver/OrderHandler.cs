using System;
using System.Threading;
using Messages;
using NServiceBus;

public class OrderHandler : IHandleMessages<ClientOrder>
{
    IBus bus;

    public OrderHandler(IBus bus)
    {
        this.bus = bus;
    }

    static int count;
    #region Reply

    public void Handle(ClientOrder message)
    {
        Console.WriteLine($"{DateTime.UtcNow} Handling ClientOrder with ID {message.OrderId}");
        var clientOrderAccepted = new ClientOrderAccepted
        {
            OrderId = message.OrderId
        };
        bus.Reply(clientOrderAccepted);

        if (Interlocked.Increment(ref count) % 2 == 0) throw new InvalidOperationException("Failure!");
        Console.WriteLine("\tOK!");
    }
    #endregion
}