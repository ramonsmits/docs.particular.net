using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Store.CustomerRelations";
        LoggingConfiguration.Setup();

        var endpointConfiguration = new EndpointConfiguration("Store.CustomerRelations");
        endpointConfiguration.ApplyCommonConfiguration();
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press Escape key to exit");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
