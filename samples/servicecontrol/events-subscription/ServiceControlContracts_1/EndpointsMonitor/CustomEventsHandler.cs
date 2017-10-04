using NServiceBus;
using NServiceBus.Logging;
using ServiceControl.Contracts;

#region ServiceControlEventsHandlers

public class CustomEventsHandler :
    IHandleMessages<MessageFailed>,
    IHandleMessages<HeartbeatStopped>,
    IHandleMessages<HeartbeatRestored>
{
    static ILog log = LogManager.GetLogger<CustomEventsHandler>();

    public void Handle(MessageFailed message)
    {
        log.Error($"Received ServiceControl '{nameof(MessageFailed)}' event for a '{message.MessageType}' with ID '{message.FailedMessageId}'.");
    }

    public void Handle(HeartbeatStopped message)
    {
        log.Warn($"Heartbeat from {message.EndpointName} stopped.");
    }

    public void Handle(HeartbeatRestored message)
    {
        log.Info($"Heartbeat from {message.EndpointName} restored.");
    }
}

#endregion