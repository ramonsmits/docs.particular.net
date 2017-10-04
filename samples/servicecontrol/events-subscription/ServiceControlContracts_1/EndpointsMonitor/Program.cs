using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Hosting.Helpers;
using ServiceControl.Contracts;

class Program
{
    static IBus endpointInstance;
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


        using (endpointInstance = Bus.Create(endpointConfiguration).Start())
        {
            while (true)
            {
                Console.WriteLine("Press ESC key to finish.\nS = Subscribe\nU = Unsubscribe");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.S:
                        Subscribe();
                        break;
                    case ConsoleKey.U:
                        Unsubscribe();
                        break;
                }
            }
        }
    }

    static List<Type> types = new List<Type>()
    {
        typeof(MessageFailed),
        typeof(HeartbeatStopped),
        typeof(HeartbeatRestored)
    };

    static void Unsubscribe()
    {
        types.ForEach(t=>endpointInstance.Unsubscribe(t));
    }

    static void Subscribe()
    {
        types.ForEach(t => endpointInstance.Subscribe(t));
    }
}