using System;
using NServiceBus;

public class OrderSubmittedHandler : IHandleMessages<SubmitOrder>
{
    public void Handle(SubmitOrder message)
    {
        Runner.Signal();
    }
}
