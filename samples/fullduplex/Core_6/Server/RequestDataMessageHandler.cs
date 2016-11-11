using System;
using NServiceBus;
using System.Threading.Tasks;
using NHibernate;
using NServiceBus.Logging;

#region RequestDataMessageHandler

class RequestDataMessageHandler :
    IHandleMessages<RequestDataMessage>
#endregion
{
    static ILog log = LogManager.GetLogger<RequestDataMessageHandler>();

    public ISession Session { get; set; }
    public QuerySession UncommittedSession { get; set; }

    public async Task Handle(RequestDataMessage message, IMessageHandlerContext context)
    {
        log.Info($"Received request {message.DataId}.");

        var readEntity = UncommittedSession.Session.Get<Customer>(message.DataId);
        var entity = Session.Get<Customer>(message.DataId);

        UncommittedSession.Session.Save(new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Ramon"
        });

        if (null == entity)
        {
            
        }
        #region DataResponseReply

        var response = new DataResponseMessage
        {
            DataId = entity?.Id,
            String = entity?.Name
        };

        await context.Reply(response)
            .ConfigureAwait(false);

        #endregion
    }

}