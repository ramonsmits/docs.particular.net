using System;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using Environment = NHibernate.Cfg.Environment;

class Program
{
    static async Task Main()
    {
        LogManager.Use<DefaultFactory>().Level(LogLevel.Debug);

        Console.Title = "Samples.NHibernate.Server";

        #region Config

        var endpointConfiguration = new EndpointConfiguration("Samples.NHibernate.Server");

        var persistence = endpointConfiguration.UsePersistence<NHibernatePersistence>();


        var nhConfig = new Configuration();
        nhConfig.SetProperty(Environment.ConnectionProvider, typeof(NHibernate.Connection.DriverConnectionProvider).FullName);
        nhConfig.SetProperty(Environment.ConnectionDriver, typeof(NHibernate.Driver.Sql2008ClientDriver).FullName);
        nhConfig.SetProperty(Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).FullName);
        nhConfig.SetProperty(Environment.ConnectionStringName, "NServiceBus/Persistence");
        nhConfig.SetProperty(Environment.Isolation, nameof(System.Data.IsolationLevel.Snapshot));

        AddMappings(nhConfig);

        persistence.UseConfiguration(nhConfig);

        #endregion

        var t = endpointConfiguration.UseTransport<MsmqTransport>();
        endpointConfiguration.SendFailedMessagesTo("error");
        t.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
        t.TransactionScopeOptions(TimeSpan.FromMinutes(1), System.Transactions.IsolationLevel.Snapshot);


        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static void AddMappings(Configuration nhConfiguration)
    {
        var mapper = new ModelMapper();
        mapper.AddMappings(typeof(OrderShipped).Assembly.GetTypes());
        nhConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
    }
}