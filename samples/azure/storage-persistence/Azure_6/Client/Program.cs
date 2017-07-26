using System;
using NServiceBus;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.Azure.StoragePersistence.Client";
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.Azure.StoragePersistence.Client");
        busConfiguration.ApplyGlobalSettings();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press 'enter' to send a StartOrder messages");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey().Key;
                Console.WriteLine();

                if (key == ConsoleKey.Escape)
                {
                    return;
                }

                if (key == ConsoleKey.S)
                {
                    bus.Subscribe<OrderCompleted>();
                    continue;
                }

                if (key == ConsoleKey.U)
                {
                    bus.Unsubscribe<OrderCompleted>();
                    continue;
                }

                var orderId = Guid.NewGuid();
                var startOrder = new StartOrder
                {
                    OrderId = orderId
                };
                bus.Send(startOrder);
                Console.WriteLine($"StartOrder Message sent with OrderId {orderId}");
            }
        }
    }
}