using System;
using NServiceBus;

class Program
{

    static void Main()
    {
        Console.Title = "Server";

        #region server
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.Scaleout.v5.Server");
        busConfiguration.RunMSMQDistributor(withWorker: false);
        #endregion
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}