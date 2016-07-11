using NServiceBus;
using System.Threading.Tasks;
using NServiceBus.Logging;

#region RequestDataMessageHandler

public class RequestDataMessageHandler : IHandleMessages<RequestDataMessage>
#endregion
{
    static ILog log = LogManager.GetLogger<RequestDataMessageHandler>();

    public async Task Handle(RequestDataMessage message, IMessageHandlerContext context)
    {
        log.Info($"Received request {message.DataId}.");
        log.Info($"String received: {message.String}.");

        #region DataResponseReply

        await context.Reply<DataResponseMessage>(response =>
        {
            response.DataId = message.DataId;
            response.String = message.String;
        })
            .ConfigureAwait(false);

        #endregion
    }

}