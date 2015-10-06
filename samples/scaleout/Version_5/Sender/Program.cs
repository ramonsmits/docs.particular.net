using System;
using NServiceBus;

class Program
{

    static void Main()
    {
        Console.Title = "Sender";
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.Scaleout.v5.Sender");
        busConfiguration.EnableInstallers();
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press 'Enter' to send a message.");
            Console.WriteLine("Press any other key to exit.");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
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

        PlaceOrder placeOrder = new PlaceOrder
        {
            OrderId = Guid.NewGuid()
        };
        bus.Send("Samples.Scaleout.v5.Server", placeOrder);
        Console.WriteLine("Sent PlacedOrder command with order id [{0}].", placeOrder.OrderId);

        #endregion
    }
}