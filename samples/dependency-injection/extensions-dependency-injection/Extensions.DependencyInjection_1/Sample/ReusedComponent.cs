using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.UniformSession;

class ReusedComponent
{
    IUniformSession session;

    public ReusedComponent(IUniformSession session)
    {
        this.session = session;
    }

    public Task Do()
    {
        Console.WriteLine("Do it!");
        var options = new SendOptions();
        options.RouteToThisEndpoint();
        options.DelayDeliveryWith(TimeSpan.FromSeconds(5));
        return session.Send(new MyMessage(), options);
    }
}