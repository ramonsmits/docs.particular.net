using System;
using NServiceBus;
using NHibernate;

public class OrderSubmittedHandler : IHandleMessages<SubmitOrder>
{
    IBus bus;
    ISession session;
    
    public OrderSubmittedHandler(IBus bus ,ISession session)
    {
        this.bus = bus;
        this.session = session;
    }

    public void Handle(SubmitOrder message)
    {
        //Console.WriteLine("Order {0} worth {1} submitted", message.OrderId, message.Value);

        Program.X.Signal();

        //session.SaveOrUpdate(new Order
        //{
        //    OrderId = message.OrderId,
        //    Value = message.Value
        //});

        //bus.Reply(new OrderAccepted
        //{
        //    OrderId = message.OrderId,
        //});

    }
}