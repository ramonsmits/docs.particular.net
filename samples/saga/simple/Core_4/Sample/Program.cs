﻿using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Installation.Environments;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.ComplexSagaFindingLogic";
        Configure.Serialization.Json();
        Configure.Features.Enable<Sagas>();
        var configure = Configure.With();
        configure.UnityBuilder();
        configure.Log4Net();
        configure.DefineEndpointName("Samples.ComplexSagaFindingLogic");
        configure.DefaultBuilder();
        configure.InMemorySagaPersister();
        configure.UseInMemoryTimeoutPersister();
        configure.InMemorySubscriptionStorage();

        configure.UseTransport<Msmq>();
        configure.Configurer.ConfigureComponent<Test>(DependencyLifecycle.InstancePerCall);

        using (var startableBus = configure.UnicastBus().CreateBus())
        {
            var bus = startableBus.Start(() => configure.ForInstallationOn<Windows>().Install());

            var startOrder1 = new StartOrder
            {
                OrderId = "123"
            };
            bus.SendLocal(startOrder1);
            var startOrder2 = new StartOrder
            {
                OrderId = "456"
            };
            bus.SendLocal(startOrder2);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}