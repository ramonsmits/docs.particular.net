using System;
using System.Data;
using NHibernate;
using NServiceBus;
using NHibernate.Cfg;
using NHibernate.Mapping.Attributes;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

internal static class EndpointConfigurationExtensions
{
    public static void ConfigureQuerySession(this EndpointConfiguration cfg)
    {
        var nhConfig = new Configuration();
        nhConfig
            .SetProperty(Environment.ConnectionStringName, "DB")
            .SetProperty(Environment.Dialect, typeof(NHibernate.Dialect.MsSql2012Dialect).FullName);

        AddAttributeMappings(nhConfig);

        new SchemaExport(nhConfig).Create(useStdOut: false, execute: true);

        var sessionFactory = nhConfig.BuildSessionFactory();

        cfg.RegisterComponents(cc =>
        {
            cc.ConfigureComponent<QuerySession>(DependencyLifecycle.InstancePerUnitOfWork);
            cc.ConfigureComponent(builder => sessionFactory, DependencyLifecycle.SingleInstance);
            cc.ConfigureComponent(builder =>
            {
                Console.WriteLine("Opening session");
                return sessionFactory.OpenSession();
            }
            , DependencyLifecycle.InstancePerUnitOfWork
            );
        });

        cfg.Pipeline.Register(builder => new UnitOfWorkBehavior(), "UnitOfWorkBehavior");

    }
    static void AddAttributeMappings(Configuration nhConfiguration)
    {
        var hbmSerializer = new HbmSerializer
        {
            Validate = true
        };

        using (var stream = hbmSerializer.Serialize(typeof(Program).Assembly))
        {
            nhConfiguration.AddInputStream(stream);
        }
    }
}
