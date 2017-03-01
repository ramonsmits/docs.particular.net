using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class LogMessageHandler : IHandleMessages<object>
{
    static readonly bool IsDebugEnabled = Log.IsDebugEnabled;
    static readonly ILog Log = LogManager.GetLogger("ReceiveStats");

    public Task Handle(object message, IMessageHandlerContext context)
    {
        if (IsDebugEnabled)
        {
            var sentAt = DateTimeExtensions.ToUtcDateTime(context.MessageHeaders[Headers.TimeSent]);
            var duration = DateTime.UtcNow - sentAt;
            Log.DebugFormat("Received message '{0}' send at '{1}' took '{2}'", context.MessageId, sentAt, duration);
        }
        return Task.FromResult(0);
    }
}
