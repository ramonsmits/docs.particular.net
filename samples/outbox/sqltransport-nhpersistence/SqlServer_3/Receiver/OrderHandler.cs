using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class OrderHandler : IHandleMessages<OrderSubmitted>
{
    static ILog Log = LogManager.GetLogger<OrderHandler>();

    public async Task Handle(OrderSubmitted message, IMessageHandlerContext context)
    {
        await Task.Delay(3000);
        const string Retries = "NServiceBus.Retries";


        // if no SLR, then always fail, if SLR then succeed
        var headers = context.MessageHeaders;

        if (!headers.ContainsKey(Retries))
        {
            throw new InvalidOperationException("boom baby!");
        }
        Log.Info("Done!");
    }
}