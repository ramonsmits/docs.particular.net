using NServiceBus;

public class NativeMessage : ICommand
{
    public string Content { get; set; }
}