using NServiceBus;

#region MessageWithLargePayload

[TimeToBeReceived("00:03:00")]
public class MessageWithLargePayload : ICommand
{
    public byte[] LargePayload { get; set; }

    public string Description { get; set; }
}

#endregion

