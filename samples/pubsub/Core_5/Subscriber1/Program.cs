using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;

static class Program
{

    static void Main()
    {
        Console.Title = "Samples.PubSub.Subscriber1";
        LogManager.Use<DefaultFactory>().Level(LogLevel.Info);
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.PubSub.Subscriber1");
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
        busConfiguration.UseTransport<RabbitMQTransport>().ConnectionString("host=localhost");
        busConfiguration.DisableFeature<SecondLevelRetries>();
        using (Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }

}