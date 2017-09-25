using System;
using System.Threading.Tasks;
using NServiceBus;

#region Handler
public class ShipOrderHandler :
    IHandleMessages<ShipOrder>
{
    public Task Handle(ShipOrder message, IMessageHandlerContext context)
    {
        var session = context.SynchronizedStorageSession.Session();
        var orderShipped = new OrderShipped
        {
            Id = message.OrderId,
            ShippingDate = DateTime.UtcNow,
        };

        
        //session.Save(orderShipped); // Ignoring as we force dupes and this results already exists exception.

        return Task.CompletedTask;
    }
}
#endregion