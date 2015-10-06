using System;
using NServiceBus;
using NServiceBus.Installation.Environments;

class Program
{
    static void Main()
    {
        Console.Title = "Server";
        Configure.Serialization.Json();

        #region server
        Configure configure = Configure.With();
        configure.Log4Net();
        configure.DefineEndpointName("Samples.Scaleout.v4.Server");
        configure.DefaultBuilder();
        configure.RunMSMQDistributor(withWorker:false);
        #endregion
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