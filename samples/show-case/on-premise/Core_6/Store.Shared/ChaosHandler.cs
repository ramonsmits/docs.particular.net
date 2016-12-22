using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class ChaosHandler : IHandleMessages<object>
{
    static readonly ILog Log = LogManager.GetLogger<ChaosHandler>();
    static readonly Random Random = new Random();
    static readonly double Thresshold = 0.8; // 80% failure

    public Task Handle(object message, IMessageHandlerContext context)
    {
        double result;

        lock (Random)
        {
            result = Random.NextDouble();
        }

        if (result < Thresshold) throw new Exception("Random chaos");
        return Task.FromResult(0);
    }
}