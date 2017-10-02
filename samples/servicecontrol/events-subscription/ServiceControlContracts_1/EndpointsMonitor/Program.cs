using System;
using NServiceBus;

class Program
{
    static void Main()
    {
        Console.Title = "EndpointsMonitor";
        var endpointConfiguration = new BusConfiguration();
        endpointConfiguration.EndpointName("EndpointsMonitor");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var transport = endpointConfiguration.UseTransport<MsmqTransport>();

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningEventsAs(
            type =>
            {
                return typeof(IEvent).IsAssignableFrom(type) ||
                       // include ServiceControl events
                       type.Namespace != null &&
                       type.Namespace.StartsWith("ServiceControl.Contracts");
            });


        using (var endpointInstance = Bus.Create(endpointConfiguration).Start())
        {
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
    }
}