using System;
using NHibernate;
using NServiceBus;

#region Handler
public class ShipOrderHandler :
    IHandleMessages<ShipOrder>
{
    ISession session;
    public IBus Bus { get; set; }

    public void Handle(ShipOrder message)
    {
        if (!message.Defer)
        {
            message.Defer = true;
            Bus.Defer(TimeSpan.FromMinutes(5), message);
            return;
        }

        
        var orderShipped = new OrderShipped
        {
            Id = message.OrderId,
            ShippingDate = DateTime.UtcNow,
        };

        session.Save(orderShipped);
    }

    public ShipOrderHandler(ISession session)
    {
        this.session = session;
    }
}
#endregion