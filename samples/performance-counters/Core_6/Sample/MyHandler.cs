using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

#region handler

public class MyHandler :
    IHandleMessages<MyMessage>
{
    static ILog log = LogManager.GetLogger<MyHandler>();

    static Random random = new Random();

    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        var delay = 1000;
        var spread = 200;
        var sleepTime = random.Next(delay - spread, delay + spread);
        log.Info($"Hello from MyHandler. Sleeping for {sleepTime}ms");
        return Task.Delay(sleepTime);
    }

}

#endregion