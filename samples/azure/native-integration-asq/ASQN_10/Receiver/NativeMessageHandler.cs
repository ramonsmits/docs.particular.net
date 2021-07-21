using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class NativeMessageHandler
    : IHandleMessages<NativeMessage>
    , IHandleMessages<NativeMessage2>

{
    static ILog log = LogManager.GetLogger<NativeMessageHandler>();

    public Task Handle(NativeMessage message, IMessageHandlerContext context)
    {
        log.Info($"Message content: {message.Content}");

        return Task.CompletedTask;
    }
    public Task Handle(NativeMessage2 message, IMessageHandlerContext context)
    {
        log.Info($"Message2 content: {message.Property}");

        return Task.CompletedTask;
    }
}