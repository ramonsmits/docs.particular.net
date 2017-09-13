using System;
using System.Threading.Tasks;
using NServiceBus;
using Store.Messages.RequestResponse;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Store.CustomerRelations";
        LoggingConfiguration.Setup();

        var endpointConfiguration = new EndpointConfiguration("Store.CustomerRelations");
        endpointConfiguration.ApplyCommonConfiguration(transport =>
        {
            var routing = transport.Routing();
            /*
              <add Assembly="Store.Messages"           Namespace="Store.Messages.Events"           Endpoint="Store.Sales"/>
              <add Assembly="Store.Messages"           Type="Store.Messages.Events.ClientBecamePreferred"           Endpoint="Store.CustomerRelations"/>
             */
            routing.RegisterPublisher(typeof(ProvisionDownloadRequest).Assembly, "Store.Messages.Events", "Store.Sales");
            routing.RegisterPublisher(typeof(Store.Messages.Events.ClientBecamePreferred), "Store.CustomerRelations");

        });
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press Escape key to exit");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
