using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.FullDuplex.Server";
        LogManager.Use<DefaultFactory>().Level(LogLevel.Debug);

        var endpointConfiguration = new EndpointConfiguration("Samples.FullDuplex.Server");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        endpointConfiguration.ConfigureQuerySession();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        try
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}

