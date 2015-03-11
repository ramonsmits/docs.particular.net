using System;
using System.Linq;
using Messages;
using NServiceBus;
using NServiceBus.Transports.SQLServer;

namespace Sender
{
	class Program
	{
		static void Main()
		{
			Console.Title = "Sender";
			const string letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
			Random random = new Random();
			BusConfiguration busConfig = new BusConfiguration();

			busConfig
				.UseTransport<SqlServerTransport>()
				.DefaultSchema("sender")
				.UseSpecificConnectionInformation(
					EndpointConnectionInfo
						.For("receiver")
						.UseSchema("receiver")
					)
				;

			busConfig
				.UsePersistence<NHibernatePersistence>();

			using (IBus bus = Bus.Create(busConfig).Start())
			{
				while (true)
				{
					Console.WriteLine("Press <enter> to send a message");
					Console.ReadLine();

					string orderId = new string(Enumerable.Range(0, 4).Select(x => letters[random.Next(letters.Length)]).ToArray());
					bus.Publish(new OrderSubmitted
					{
						OrderId = orderId,
						Value = random.Next(100)
					});
				}
			}
		}
	}
}
