using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Persistence.Legacy;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.Scaleout.Publisher";
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.Scaleout.Publisher");
        busConfiguration.EnableInstallers();
        busConfiguration.UsePersistence<MsmqPersistence>();
        busConfiguration.DisableFeature<TimeoutManager>();

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press 'Enter' to send a message.");
            Console.WriteLine("Press any other key to exit.");
            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    return;
                }

                SendMessage(bus);
            }
        }
    }

    static void SendMessage(IBus bus)
    {
        #region sender

        var placeOrder = new PlaceOrder
        {
            OrderId = Guid.NewGuid()
        };
        bus.SendLocal(placeOrder);
        Console.WriteLine($"Sent PlacedOrder command with order id [{placeOrder.OrderId}].");

        #endregion
    }
}
