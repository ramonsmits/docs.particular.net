using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

#region thesaga

public class OrderSaga :
    Saga<OrderSagaData>,
    IAmStartedByMessages<StartOrder>,
    IHandleTimeouts<CompleteOrder>
{
    static ILog log = LogManager.GetLogger<OrderSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        mapper.ConfigureMapping<StartOrder>(message => message.OrderId)
            .ToSaga(sagaData => sagaData.OrderId);
    }

    public async Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        if (!string.IsNullOrEmpty(Data.OrderDescription))
        {
            Console.WriteLine("Already started");
            return;
        }
        await context.SendLocal(new DoYourThing());

        Data.OrderId = message.OrderId;
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

        await RequestTimeout(context, TimeSpan.FromSeconds(15), timeoutData).ConfigureAwait(false);
    }

    public async Task Timeout(CompleteOrder state, IMessageHandlerContext context)
    {
        log.Info($"Saga with OrderId {Data.OrderId} completed");
        var orderCompleted = new OrderCompleted
        {
            OrderId = Data.OrderId
        };
        await context.Publish(orderCompleted)
            .ConfigureAwait(false);
        MarkAsComplete();
    }
}

class DoYourThing : ICommand
{
    
}

class DoYourThingHandler : IHandleMessages<DoYourThing>
{
    public Task Handle(DoYourThing message, IMessageHandlerContext context)
    {
        Console.WriteLine("Happy dance!");
        return Task.FromResult(0);
    }
}
#endregion