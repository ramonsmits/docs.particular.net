using System;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NServiceBus;
using NServiceBus.Logging;

static class Program
{
    static void Main()
    {
        Console.Title = "Samples.Unity";

        LogManager.Use<DefaultFactory>().Level(LogLevel.Warn);

        using (var container = new UnityContainer())
        {
            container.RegisterInstance(new MyService());

            var endpointNames = new[]
            {
                "a",
                "b",
                "c",
                "d",
                "e"
            };

            Parallel.ForEach(endpointNames, endpointName =>
            {
                var childContainer = container.CreateChildContainer();
                var busConfiguration = new BusConfiguration();
                busConfiguration.EndpointName(endpointName);
                busConfiguration.UseContainer<UnityBuilder>(
                    customizations: customizations =>
                    {
                        customizations.UseExistingContainer(childContainer);
                    });


                busConfiguration.UseSerialization<JsonSerializer>();
                busConfiguration.UsePersistence<InMemoryPersistence>();
                busConfiguration.EnableInstallers();

                var instance = Bus.Create(busConfiguration).Start();
                childContainer.RegisterInstance(instance, new ContainerControlledLifetimeManager()); // instance is incorrectly registered by NServiceBus V5

                instance.SendLocal(new MyMessage());
            });

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}