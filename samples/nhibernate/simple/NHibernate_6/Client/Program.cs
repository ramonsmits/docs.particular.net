using System;
using NHibernate.Cfg;
using NServiceBus;
using NServiceBus.Persistence;
using Environment = NHibernate.Cfg.Environment;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.NHibernate.Client";
        var busConfiguration = new BusConfiguration();

        busConfiguration.EndpointName("Samples.NHibernate.Client");
        busConfiguration.EnableInstallers();


        // Requires, otherwise it will use no schema and fail because it conflicts with  table in differen schema
        var sagaConfig = new Configuration();
        sagaConfig.SetProperty(Environment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).FullName);
        sagaConfig.SetProperty(Environment.ConnectionDriver, typeof(NHibernate.Driver.Sql2008ClientDriver).FullName);
        sagaConfig.SetProperty(Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).FullName);
        sagaConfig.SetProperty(Environment.ConnectionString, "Data Source=.;Database=Samples.NHibernate;Integrated Security=True;App=Saga");
        sagaConfig.SetProperty(Environment.DefaultSchema, "client");

        var persistence = busConfiguration.UsePersistence<NHibernatePersistence>();
        persistence.UseConfiguration(sagaConfig);


        busConfiguration.UsePersistence<NHibernatePersistence>();

        var bus = Bus.Create(busConfiguration).Start();

        Console.WriteLine("Press 'enter' to send a StartOrder messages");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId
            };

            bus.Send(startOrder);
            Console.WriteLine($"StartOrder Message sent with OrderId {orderId}");
        }

        bus.Dispose();
    }
}
