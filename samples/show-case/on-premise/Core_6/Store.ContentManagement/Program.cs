using System;
using System.Threading.Tasks;
using NServiceBus;
using Store.Messages.RequestResponse;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Store.ContentManagement";
        LoggingConfiguration.Setup();




        var endpointConfiguration = new EndpointConfiguration("Store.ContentManagement");
        endpointConfiguration.ApplyCommonConfiguration(
            transport =>
            {
                var routing = transport.Routing();
                //            < add Assembly = "Store.Messages" Namespace =  Endpoint =  />
                routing.RegisterPublisher(typeof(ProvisionDownloadRequest).Assembly, "Store.Messages.Events", "Store.Sales");
                //            < add Assembly = "Store.Messages" Namespace = "" Endpoint = "Store.Operations" />
                routing.RouteToEndpoint(typeof(ProvisionDownloadRequest).Assembly, "Store.Messages.RequestResponse", "Store.Operations");
            });

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press Escape key to exit");
        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
