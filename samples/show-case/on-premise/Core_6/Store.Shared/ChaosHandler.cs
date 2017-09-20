using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class ChaosHandler : IHandleMessages<object>
{
    static readonly ILog Log = LogManager.GetLogger<ChaosHandler>();
    static readonly Random Random = new Random();
    static readonly double Thresshold = 0.6; // 60% failure

    public Task Handle(object message, IMessageHandlerContext context)
    {
        double result;

        lock (Random)
        {
            result = Random.NextDouble();
        }

        if (result < Thresshold) throw new Exception($"Random chaos ({Thresshold * 100:N}% failure)");
        return Task.FromResult(0);
    }
}