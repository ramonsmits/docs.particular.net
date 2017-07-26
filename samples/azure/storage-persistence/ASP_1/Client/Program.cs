using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.Azure.StoragePersistence.Client";
        var endpointConfiguration = new EndpointConfiguration("Samples.Azure.StoragePersistence.Client");
        var transport = endpointConfiguration.ApplyGlobalSettings();

        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(StartOrder), "Samples.Azure.StoragePersistence.Server");
        routing.RegisterPublisher(typeof(OrderCompleted), "Samples.Azure.StoragePersistence.Server");

        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);


        Console.WriteLine("Press 'enter' to send a StartOrder messages");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey().Key;
            Console.WriteLine();

            if (key == ConsoleKey.Escape)
            {
                break;
            }

            if (key == ConsoleKey.S)
            {
                await endpointInstance.Subscribe<OrderCompleted>().ConfigureAwait(false);
                continue;
            }

            if (key == ConsoleKey.U)
            {
                await endpointInstance.Unsubscribe<OrderCompleted>().ConfigureAwait(false);
                continue;
            }

            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId
            };
            await endpointInstance.Send(startOrder)
                .ConfigureAwait(false);
            Console.WriteLine($"StartOrder Message sent with OrderId {orderId}");
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}