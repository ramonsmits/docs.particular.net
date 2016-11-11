using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.FullDuplex.Client";
        LogManager.Use<DefaultFactory>()
            .Level(LogLevel.Info);
        var endpointConfiguration = new EndpointConfiguration("Samples.FullDuplex.Client");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        try
        {
            Console.WriteLine("Press enter to send a message");
            Console.WriteLine("Press any key to exit");

            #region ClientLoop

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    return;
                }
                var guid = Guid.NewGuid();
                Console.WriteLine($"Requesting to get data by id: {guid:N}");

                var message = new RequestDataMessage
                {
                    DataId = guid,
                };
                await endpointInstance.Send("Samples.FullDuplex.Server", message)
                    .ConfigureAwait(false);
            }

            #endregion
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}