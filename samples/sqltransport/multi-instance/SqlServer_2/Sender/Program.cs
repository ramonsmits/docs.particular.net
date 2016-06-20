﻿using System;
using Messages;
using NServiceBus;

public class Program
{
    static void Main()
    {
        Console.Title = "Samples.SqlServer.MultiInstanceSender";

        #region SenderConfiguration

        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.SqlServer.MultiInstanceSender");
        var transport = busConfiguration.UseTransport<SqlServerTransport>();
        transport.UseSpecificConnectionInformation(ConnectionProvider.GetConnection);
        transport.ConnectionString(ConnectionProvider.SenderConnectionString);
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        #endregion

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any key to send a message");
            Console.WriteLine("Press ESC  key to exit");

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    return;
                }

                #region SendMessage

                var order = new ClientOrder
                {
                    OrderId = Guid.NewGuid()
                };

                bus.Send("Samples.SqlServer.MultiInstanceReceiver", order);

                #endregion

                Console.WriteLine($"ClientOrder message sent with ID {order.OrderId}");
            }
        }
    }

}