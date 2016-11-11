using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class BoomEventMessageHandler :
    IHandleMessages<IMyEvent>
{
    static ILog log = LogManager.GetLogger<BoomEventMessageHandler>();

    public Task Handle(IMyEvent message, IMessageHandlerContext context)
    {
        throw new Exception("Error!");
    }

}