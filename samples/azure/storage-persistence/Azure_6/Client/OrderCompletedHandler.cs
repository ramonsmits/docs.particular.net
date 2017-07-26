using NServiceBus;
using NServiceBus.Logging;

public class OrderCompletedHandler :
    IHandleMessages<OrderCompleted>
{
    static ILog log = LogManager.GetLogger<OrderCompletedHandler>();
    public IBus Bus { get; set; }

    public void Handle(OrderCompleted message)
    {
        log.Info($"Received OrderCompleted {Bus.GetMessageHeader(message, Headers.MessageId)} for OrderId {message.OrderId}");
    }
}