﻿using System;
using NServiceBus;
using NServiceBus.DataBus;

class Program
{
    static void Main()
    {
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.AzureBlobStorageDataBus.Sender");
        busConfiguration.UseSerialization<JsonSerializer>();

        #region ConfiguringDataBusLocation

        busConfiguration.UseDataBus<AzureDataBus>()
            .ConnectionString("UseDevelopmentStorage=true");

        #endregion

        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
        busConfiguration.Conventions()
            .DefiningDataBusPropertiesAs(p => p.PropertyType == typeof(byte[]));

        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Run(bus);
        }
    }

    static void Run(IBus bus)
    {
        Console.WriteLine("Press 'Enter' to send a large message (>4MB)");
        Console.WriteLine("To exit, press Ctrl + C");

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                SendMessageLargePayload(bus);
            }
        }
    }

    static void SendMessageLargePayload(IBus bus)
    {
        Console.WriteLine("Sending message...");

        #region SendMessageLargePayload

        MessageWithLargePayload message = new MessageWithLargePayload
        {
            Description = "This message contains a large payload that will be sent on the Azure data bus",
            LargePayload = new byte[1024*1024*5] // 5MB
        };
        bus.Send("Samples.AzureBlobStorageDataBus.Receiver", message);

        #endregion

        Console.WriteLine("Message sent.");
    }
}