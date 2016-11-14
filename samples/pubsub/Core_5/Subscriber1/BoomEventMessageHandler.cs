using System;
using NServiceBus;

public class BoomEventMessageHandler : IHandleMessages<IMyEvent>
{
    public void Handle(IMyEvent message)
    {
        throw new Exception("Error!");
    }
}