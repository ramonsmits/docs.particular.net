using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.UniformSession;

#region InjectingMessageSession
public class MessageSenderService
{
    private readonly IUniformSession messageSession;

    public MessageSenderService(IUniformSession messageSession)
    {
        this.messageSession = messageSession;
    }

    public Task SendMessage()
    {
        var myMessage = new MyMessage();
        return messageSession.SendLocal(myMessage);
    }
}
#endregion