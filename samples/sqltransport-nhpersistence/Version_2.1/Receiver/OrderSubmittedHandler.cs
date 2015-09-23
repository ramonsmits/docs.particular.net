using System;
using NServiceBus;
using NHibernate;

public class OrderSubmittedHandler : IHandleMessages<SubmitOrder>
{
    public void Handle(SubmitOrder message)
    {
        Runner.X.Signal();
    }
}
