﻿using System;
using NServiceBus;
using NServiceBus.Installation.Environments;

class Program
{
    static void Main()
    {
        Console.Title = "Sender";
        Configure.Serialization.Json();
        Configure configure = Configure.With();
        configure.Log4Net();
        configure.DefineEndpointName("Samples.Scaleout.v4.Sender");
        configure.DefaultBuilder();
        configure.InMemorySagaPersister();
        configure.UseInMemoryTimeoutPersister();
        configure.InMemorySubscriptionStorage();
        configure.UseTransport<Msmq>();
        using (IStartableBus startableBus = configure.UnicastBus().CreateBus())
        {
            IBus bus = startableBus.Start(() => configure.ForInstallationOn<Windows>().Install());

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
        bus.Send("Samples.Scaleout.v4.Server", placeOrder);
        Console.WriteLine("Sent PlacedOrder command with order id [{0}].", placeOrder.OrderId);

        #endregion
    }
}