using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Store.Messages.Commands;
using Store.Messages.Events;

public class ProcessOrderSaga :
    Saga<ProcessOrderSaga.OrderData>,
    IAmStartedByMessages<SubmitOrder>,
    IHandleMessages<CancelOrder>,
    IHandleTimeouts<ProcessOrderSaga.BuyersRemorseIsOver>
{
    static ILog log = LogManager.GetLogger<ProcessOrderSaga>();
    static readonly TimeSpan BuyersRemorseDuration = TimeSpan.FromSeconds(5);

    public Task Handle(SubmitOrder message, IMessageHandlerContext context)
    {
        if (DebugFlagMutator.Debug)
        {
            Debugger.Break();
        }

        Data.OrderNumber = message.OrderNumber;
        Data.ProductIds = message.ProductIds;
        Data.ClientId = message.ClientId;

        log.InfoFormat($"Starting cool down period of {1} for order #{0}.", Data.OrderNumber, BuyersRemorseDuration);
        return RequestTimeout(context, BuyersRemorseDuration, new BuyersRemorseIsOver());
    }

    public Task Timeout(BuyersRemorseIsOver state, IMessageHandlerContext context)
    {
        if (DebugFlagMutator.Debug)
        {
            Debugger.Break();
        }

        log.Info($"Cooling down period for order #{Data.OrderNumber} has elapsed.");

        var orderAccepted = new OrderAccepted
        {
            OrderNumber = Data.OrderNumber,
            ProductIds = Data.ProductIds,
            ClientId = Data.ClientId
        };

        MarkAsComplete();
        return context.Publish(orderAccepted);
    }

    public Task Handle(CancelOrder message, IMessageHandlerContext context)
    {
        if (DebugFlagMutator.Debug)
        {
            Debugger.Break();
        }

        MarkAsComplete();

        log.Info($"Order #{message.OrderNumber} was cancelled.");

        var orderCancelled = new OrderCancelled
        {
            OrderNumber = message.OrderNumber,
            ClientId = message.ClientId
        };
        return context.Publish(orderCancelled);
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderData> mapper)
    {
        mapper.ConfigureMapping<SubmitOrder>(message => message.OrderNumber)
            .ToSaga(sagaData => sagaData.OrderNumber);
        mapper.ConfigureMapping<CancelOrder>(message => message.OrderNumber)
            .ToSaga(sagaData => sagaData.OrderNumber);
    }

    public class OrderData :
        ContainSagaData
    {
        public virtual int OrderNumber { get; set; }
        public virtual string[] ProductIds { get; set; }
        public virtual string ClientId { get; set; }
    }

    public class BuyersRemorseIsOver
    {
    }

}
