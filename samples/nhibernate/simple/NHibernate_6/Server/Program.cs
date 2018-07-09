using System;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBus.Persistence.NHibernate;
using Environment = NHibernate.Cfg.Environment;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.NHibernate.Server";

        #region Config

        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.NHibernate.Server");

        var persistence = busConfiguration.UsePersistence<NHibernatePersistence>();
        var sagaConfig = new Configuration();
        sagaConfig.SetProperty(Environment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).FullName);
        sagaConfig.SetProperty(Environment.ConnectionDriver, typeof(NHibernate.Driver.Sql2008ClientDriver).FullName);
        sagaConfig.SetProperty(Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).FullName);
        sagaConfig.SetProperty(Environment.ConnectionString, "Data Source=.;Database=Samples.NHibernate;Integrated Security=True;App=Saga");
        sagaConfig.SetProperty(Environment.DefaultSchema, "saga");

        AddMappings(sagaConfig);
        persistence.UseConfiguration(sagaConfig).RegisterManagedSessionInTheContainer();

        var timeoutConfig = new Configuration();
        timeoutConfig.SetProperty(Environment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).FullName);
        timeoutConfig.SetProperty(Environment.ConnectionDriver, typeof(NHibernate.Driver.Sql2008ClientDriver).FullName);
        timeoutConfig.SetProperty(Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).FullName);
        timeoutConfig.SetProperty(Environment.ConnectionString, "Data Source=.;Database=Samples.NHibernate;Integrated Security=True;App=Timeout");
        timeoutConfig.SetProperty(Environment.DefaultSchema, "timeout");

        persistence.UseTimeoutStorageConfiguration(timeoutConfig);

        #endregion

        busConfiguration.EnableInstallers();

        var bus = Bus.Create(busConfiguration).Start();

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        bus.Dispose();
    }

    static void AddMappings(Configuration nhConfiguration)
    {
        var mapper = new ModelMapper();
        mapper.AddMappings(typeof(OrderShipped).Assembly.GetTypes());
        nhConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
    }
}
