using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Store.Operations";
        LoggingConfiguration.Setup();

        var endpointConfiguration = new EndpointConfiguration("Store.Operations");
        endpointConfiguration.ApplyCommonConfiguration();
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press Escape key to exit");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
