using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class DelayHandler : IHandleMessages<object>
{
    static readonly ILog Log = LogManager.GetLogger<DelayHandler>();
    static readonly Random Random = new Random();

    public Task Handle(object message, IMessageHandlerContext context)
    {
        int duration;

        lock (Random)
        {
            duration = Random.Next(200, 2000);
        }

        Log.InfoFormat("Delaying for {0}ms", duration);

        return Task.Delay(duration);
    }
}