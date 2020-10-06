using System;
using System.Threading.Tasks;
using NServiceBus;
#region InjectingDependency
class MyHandler :
    IHandleMessages<MyMessage>
{
    ReusedComponent myService;

    public MyHandler(ReusedComponent myService)
    {
        this.myService = myService;
    }

    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        //var timestamp = DateTimeExtensions.ToUtcDateTime(context.MessageHeaders[Headers.TimeSent]);

        //var age = DateTime.UtcNow - timestamp;

        //Console.WriteLine($"Age: {age}");

        //if (age > TimeSpan.FromSeconds(10))
        //{
        //    Console.WriteLine($"Too old {context.MessageId}");
        //    return Task.CompletedTask;
        //}

        myService.Do();
        return Task.CompletedTask;
    }
}
#endregion
