using System;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NServiceBus;
using NServiceBus.Persistence;
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
        var nhConfig = new Configuration();

        nhConfig.SetProperty(Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
        nhConfig.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.Sql2008ClientDriver");
        //nhConfig.SetProperty(Environment.Dialect, "NHibernate.Dialect.MsSql2008Dialect");
        //nhConfig.SetProperty(Environment.ConnectionStringName, "NServiceBus/Persistence");
        //nhConfig.SetProperty(Environment.ConnectionProvider, )
        nhConfig.DataBaseIntegration(x =>
        {
            x.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString;
            x.Dialect<MsSql2012Dialect>();
            x.ConnectionProvider<CustomDriverConnectionProvider>();
        });


        AddMappings(nhConfig);

        persistence.UseConfiguration(nhConfig).RegisterManagedSessionInTheContainer();

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
        mapper.AddMappings(typeof (OrderShipped).Assembly.GetTypes());
        nhConfiguration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
    }
}

public class CustomDriverConnectionProvider : NHibernate.Connection.DriverConnectionProvider

{

    public override System.Data.IDbConnection GetConnection()

    {

        var conn = base.GetConnection();

        DisableNoCount(conn);

        return conn;

    }

 

    private void DisableNoCount(System.Data.IDbConnection connection)

    {

        using (var command = connection.CreateCommand())

        {

            command.CommandText = "SET NOCOUNT OFF";

            command.ExecuteNonQuery();

        }

    }

}