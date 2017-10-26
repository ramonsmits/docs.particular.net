using System;
using NServiceBus;

[EndpointName("Samples.Scaleout.Subscriber.Worker2")]
class Program : IConfigureThisEndpoint
{
    public void Customize(BusConfiguration busConfiguration)
    {
        Console.Title = "Samples.Scaleout.Worker2";
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
    }
}