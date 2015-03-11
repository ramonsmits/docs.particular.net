namespace Receiver
{
	using System;
	using NHibernate.Cfg;
	using NHibernate.Dialect;
	using NHibernate.Mapping.ByCode;
	using NHibernate.Tool.hbm2ddl;
	using NServiceBus;
	using NServiceBus.Persistence;
	using NServiceBus.Transports.SQLServer;

	class Program
	{
		static void Main()
		{
			Console.Title = "Receiver";

			Configuration hibernateConfig = new Configuration();

			hibernateConfig.DataBaseIntegration(x =>
			{
				x.ConnectionStringName = "NServiceBus/Persistence";
				x.Dialect<MsSql2012Dialect>();
			});

			//hibernateConfig
			//	.SetProperty("default_schema", "receiver");

			ModelMapper mapper = new ModelMapper();
			mapper.AddMapping<OrderMap>();
			hibernateConfig.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			new SchemaExport(hibernateConfig).Execute(false, true, false);


			BusConfiguration busConfig = new BusConfiguration();
			busConfig
				.UseTransport<SqlServerTransport>()
				//.DefaultSchema("receiver")
				//.UseSpecificConnectionInformation(endpoint =>
				//{
				//	string schema = endpoint.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();
				//	return ConnectionInfo.Create().UseSchema(schema);
				//})
				;
			busConfig
				.UsePersistence<NHibernatePersistence>()
				.UseConfiguration(hibernateConfig)
				;

			using (Bus.Create(busConfig).Start())
			{
				Console.WriteLine("Press <enter> to exit");
				Console.ReadLine();
			}
		}
	}
}
