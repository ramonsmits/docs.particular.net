using System;
using NServiceBus;

public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
{
    public void Handle(PlaceOrder message)
    {
        Program.x.Signal();
    }
}
