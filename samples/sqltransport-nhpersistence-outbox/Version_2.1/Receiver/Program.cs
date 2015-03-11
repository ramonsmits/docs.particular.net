using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Transports.SQLServer;

namespace Receiver
{
	using System.Configuration;
	using Configuration = NHibernate.Cfg.Configuration;

	class Program
	{
		public static ISessionFactory SessionFactory;

		static void Main()
		{
			Configuration hibernateConfig = new Configuration();
			hibernateConfig.DataBaseIntegration(x =>
			{
				x.ConnectionStringName = "NServiceBus/Persistence";
				x.Dialect<MsSql2012Dialect>();
			});

			ModelMapper mapper = new ModelMapper();
			mapper.AddMapping<OrderMap>();
			hibernateConfig.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
			SessionFactory = hibernateConfig.BuildSessionFactory();

			new SchemaExport(hibernateConfig).Execute(false, true, false);

			BusConfiguration busConfig = new BusConfiguration();
			busConfig.UseTransport<SqlServerTransport>();
			busConfig.UsePersistence<NHibernatePersistence>();
			busConfig.EnableOutbox();
			busConfig.DisableFeature<SecondLevelRetries>();

			using (Bus.Create(busConfig).Start())
			{
				Console.WriteLine("Press <enter> to exit");
				Console.ReadLine();
			}
		}
	}
}