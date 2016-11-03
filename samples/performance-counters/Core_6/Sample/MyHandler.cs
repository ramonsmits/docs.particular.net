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
        //var spread = Math.Min(500, Program.processDelay);
        //var sleepTime = random.Next(Program.processDelay - spread, Program.processDelay + spread);
        var sleepTime = Program.processDelay;
        //log.Info($"Hello from MyHandler. Sleeping for {sleepTime}ms");
        Console.Write("■");
        return Task.Delay(sleepTime);
    }

}

#endregion