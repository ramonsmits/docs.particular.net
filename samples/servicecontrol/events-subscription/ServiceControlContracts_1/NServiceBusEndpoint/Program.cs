using System;
using NServiceBus;

class Program
{
    static void Main()
    {
        Console.Title = "NServiceBusEndpoint";
        var endpointConfiguration = new BusConfiguration();
        endpointConfiguration.EndpointName("NServiceBusEndpoint");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        using (var endpointInstance = Bus.Create(endpointConfiguration).Start())
        {
            Console.WriteLine("Press 'Enter' to send a new message. Press any other key to finish.");
            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }

                var guid = Guid.NewGuid();

                var simpleMessage = new SimpleMessage
                {
                    Id = guid
                };
                endpointInstance.Send("NServiceBusEndpoint", simpleMessage);

                Console.WriteLine($"Sent a new message with Id = {guid}.");

                Console.WriteLine("Press 'Enter' to send a new message. Press any other key to finish.");
            }
        }
    }
}