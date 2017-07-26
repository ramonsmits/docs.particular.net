using System;
using NServiceBus;

class Program
{

    static void Main()
    {
        Console.Title = "Samples.Azure.StoragePersistence.Server";
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.Azure.StoragePersistence.Server");
        busConfiguration.ApplyGlobalSettings();
        busConfiguration.UsePersistence<AzureStoragePersistence>();

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}