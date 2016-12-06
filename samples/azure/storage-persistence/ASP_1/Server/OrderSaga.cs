using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

#region thesaga

public class OrderSaga :
    Saga<OrderSagaData>,
    IAmStartedByMessages<StartOrder>,
    IHandleMessages<DoYourThingResponse>
{
    static ILog log = LogManager.GetLogger<OrderSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        mapper.ConfigureMapping<StartOrder>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
        mapper.ConfigureMapping<DoYourThingResponse>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
    }

    public async Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        Data.Count++;

        if (Data.Count > 1)
        {
            return; // Means that we are already processing, we need to wait until that completed.
        }

        Data.OrderId = message.OrderId;

        await context.SendLocal(new DoYourThing { OrderId = Data.OrderId });

        var orderDescription = $"The saga for order {message.OrderId}";
        Data.OrderDescription = orderDescription;
        log.Info($"Received StartOrder message {Data.OrderId}. Starting Saga");
        log.Info("Order will complete in 15 seconds");
        var timeoutData = new CompleteOrder
        {
            OrderDescription = orderDescription
        };

        await Task.Delay(1000).ConfigureAwait(false);
        Console.WriteLine("Done");
    }

    public async Task Handle(DoYourThingResponse message, IMessageHandlerContext context)
    {
        Data.Count--;
        if (Data.Count == 0)
        {
            // No more work to do
            Console.WriteLine("Done!");
            MarkAsComplete();
        }
        else
        {
            // Task for ID buffered, invoking again.
            await context.SendLocal(new DoYourThing { OrderId = Data.OrderId });
        }
    }
}

public class DoYourThing : ICommand
{
    public Guid OrderId { get; set; }
}

public class DoYourThingResponse : IMessage
{
    public Guid OrderId { get; set; }
}

class DoYourThingHandler : IHandleMessages<DoYourThing>
{
    public async Task Handle(DoYourThing message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Happy dance starting for {message.OrderId}!");
        await Task.Delay(3000).ConfigureAwait(false);
        await context.Reply(new DoYourThingResponse { OrderId = message.OrderId }).ConfigureAwait(false);
        Console.WriteLine($"Happy dance completed for {message.OrderId}!");
    }
}
#endregion