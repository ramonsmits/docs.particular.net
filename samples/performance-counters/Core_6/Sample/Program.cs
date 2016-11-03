using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static IEndpointInstance endpointInstance;

    static async Task AsyncMain()
    {
        Console.Title = "Samples.PerfCounters";
        var endpointConfiguration = new EndpointConfiguration("Samples.PerfCounters");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");

        #region enable-counters
        endpointConfiguration.PurgeOnStartup(true);
        endpointConfiguration.EnableCriticalTimePerformanceCounter();
        endpointConfiguration.EnableSLAPerformanceCounter(TimeSpan.FromSeconds(100));
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);
        #endregion

        endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        try
        {
            Console.WriteLine("Press UP/DOWN to increment/decrement send delay");
            Console.WriteLine("Press any key to exit");

            var loop = Task.Run(Loop);

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if(delay>100) delay -= 100;
                        Console.WriteLine($"Delay: {delay}ms");
                        break;
                    case ConsoleKey.DownArrow:
                        delay += 100;
                        Console.WriteLine($"Delay: {delay}ms");
                        break;
                    default:
                        Console.WriteLine(key.Key);
                        return;
                }
            }
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }


    static int delay = 1000;

    static async Task Loop()
    {
        while (true)
        {
            Console.WriteLine("Sending message");
            await endpointInstance.SendLocal(new MyMessage());
            await Task.Delay(delay).ConfigureAwait(false);
        }
    }
}
