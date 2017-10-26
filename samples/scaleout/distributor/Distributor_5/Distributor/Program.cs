using System;
using NServiceBus;

[EndpointName("Samples.Scaleout.Subscriber")]
class Program : IConfigureThisEndpoint
{
    public void Customize(BusConfiguration busConfiguration)
    {
        Console.Title = "Samples.Scaleout.Subscriber";
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
    }
}