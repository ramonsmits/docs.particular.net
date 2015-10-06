using System;
using NServiceBus;
using NServiceBus.Installation.Environments;

class Program
{
    static void Main()
    {
        Console.Title = "Worker #2";
        Configure.Serialization.Json();
        Configure configure = Configure.With();
        configure.Log4Net();
        configure.DefineEndpointName("Samples.Scaleout.v4.Worker2");
        configure.DefaultBuilder();
        configure.EnlistWithMSMQDistributor();
        Address.InitializeLocalAddress("Samples.Scaleout.v4.Worker2");
        configure.InMemorySagaPersister();
        configure.UseInMemoryTimeoutPersister();
        configure.InMemorySubscriptionStorage();
        configure.UseTransport<Msmq>();
        using (IStartableBus startableBus = configure.UnicastBus().CreateBus())
        {
            startableBus.Start(() => configure.ForInstallationOn<Windows>().Install());

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
