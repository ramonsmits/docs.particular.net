using System;
using NServiceBus;

[EndpointName("Samples.Scaleout.Subscriber.Worker1")]
class Program : IConfigureThisEndpoint
{
    public void Customize(BusConfiguration busConfiguration)
    {
        Console.Title = "Samples.Scaleout.Worker1";
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
    }
}