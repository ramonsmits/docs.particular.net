using System;
using NServiceBus;
using NServiceBus.Logging;

public class SimpleMessageHandler :
    IHandleMessages<SimpleMessage>
{
    static ILog log = LogManager.GetLogger<SimpleMessageHandler>();

    public void Handle(SimpleMessage message)
    {
        log.Info($"Received message with Id = {message.Id}.");
        throw new Exception("BOOM!");
    }
}