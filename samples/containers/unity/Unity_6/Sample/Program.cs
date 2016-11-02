using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using NServiceBus;

static class Program
{
    static void Main()
    {
        Console.Title = "Samples.Unity";


        var container = new UnityContainer();
        container.RegisterInstance(new MyService());

        var endpointNames = new[]
        {
            "a",
            "b"
        };

        var instances = new List<IBus>();
        foreach (var endpointName in endpointNames)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(endpointName);
            busConfiguration.UseContainer<UnityBuilder>(
                customizations: customizations =>
                {
                    customizations.UseExistingContainer(container.CreateChildContainer());
                });


            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();

            var instance = Bus.Create(busConfiguration).Start();
            instances.Add(instance);
        }

        Console.WriteLine("Sending local message on each instance...");

        foreach (var instance in instances)
        {
            var myMessage = new MyMessage();
            instance.SendLocal(myMessage);
        }

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        foreach (var instance in instances) instance.Dispose();
    }
}