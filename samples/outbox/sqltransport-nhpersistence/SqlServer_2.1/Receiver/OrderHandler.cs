using System;
using System.Threading;
using NServiceBus;
using NServiceBus.Logging;

public class OrderHandler : IHandleMessages<OrderSubmitted>
{
    static ILog Log = LogManager.GetLogger<OrderHandler>();
    public IBus Bus { get; set; }
    public void Handle(OrderSubmitted message)
    {
        Thread.Sleep(3000);
        const string Retries = "NServiceBus.Retries";

        // if no SLR, then always fail, if SLR then succeed
        var headers = Bus.CurrentMessageContext.Headers;

        if (!headers.ContainsKey(Retries))
        {
            throw new InvalidOperationException("boom baby!");
        }
        Log.Info("Done!");
    }
}