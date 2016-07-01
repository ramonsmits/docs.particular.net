using System;
using log4net;
using NServiceBus;
using NServiceBus.Saga;

#region thesaga
public class OrderSaga : Saga<OrderSagaData>,
    IAmStartedByMessages<StartOrder>,
    IHandleMessages<CompleteOrder>,
    IHandleTimeouts<CancelOrder>
{
    static ILog log = LogManager.GetLogger(typeof(OrderSaga));
    public ITest Test { get; set; }

    public override void ConfigureHowToFindSaga()
    {
        ConfigureMapping<StartOrder>(message => message.OrderId)
            .ToSaga(sagaData => sagaData.OrderId);
        ConfigureMapping<CompleteOrder>(message => message.OrderId)
            .ToSaga(sagaData => sagaData.OrderId);
    }

    public void Handle(StartOrder message)
    {
        Test.Call();

        Data.OrderId = message.OrderId;
        log.Info($"Saga with OrderId {Data.OrderId} received StartOrder with OrderId {message.OrderId}");
        var completeOrder = new CompleteOrder
        {
            OrderId = Data.OrderId
        };
        Bus.SendLocal(completeOrder);
        RequestTimeout<CancelOrder>(TimeSpan.FromMinutes(30));
    }

    public void Handle(CompleteOrder message)
    {
        log.Info($"Saga with OrderId {Data.OrderId} received CompleteOrder with OrderId {message.OrderId}");
        MarkAsComplete();
    }

    public void Timeout(CancelOrder state)
    {
        log.Info($"Complete not received soon enough OrderId {Data.OrderId}");
        MarkAsComplete();
    }

}

public interface ITest
{
    void Call();
}

public class Test : ITest
{
    public void Call()
    {
        Console.WriteLine("Call");
        
    }
}
#endregion