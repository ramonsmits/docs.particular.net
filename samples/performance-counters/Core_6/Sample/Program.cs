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

    static int ReadValue(int defaultValue)
    {
        var v = Console.ReadLine();
        return v.Length == 0 ? defaultValue : int.Parse(v);
    }

    static async Task AsyncMain()
    {
        var concurrency = 1;
        Console.Write($"Concurrency [{concurrency}]:");
        concurrency = ReadValue(concurrency);
        //Console.WriteLine();

        var sla = 100;
        Console.Write($"SLA in seconds [{sla}]:");
        sla = ReadValue(sla);
        //Console.WriteLine();



        var endpointConfiguration = new EndpointConfiguration("Samples.PerfCounters");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");

        endpointConfiguration.UseTransport<MsmqTransport>().ConnectionString("journal=false;deadLetter=false");
        #region enable-counters
        Console.Write("Purge? [Y]:");
        var key = Console.ReadKey().Key;
        if (key == ConsoleKey.Y || key == ConsoleKey.Enter) endpointConfiguration.PurgeOnStartup(true);
        Console.WriteLine();

        endpointConfiguration.EnableCriticalTimePerformanceCounter();
        endpointConfiguration.EnableSLAPerformanceCounter(TimeSpan.FromSeconds(sla));
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(concurrency);
        #endregion

        endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        try
        {
            Console.WriteLine("Press UP/DOWN to increment/decrement send delay");
            Console.WriteLine("Press RIGHT/LEFT to increment/decrement processing delay");
            Console.WriteLine("Press +/- to increment/decrement step size");
            Console.WriteLine("Press any key to exit");

            var loop = Task.Run(Loop);

            while (true)
            {
                Console.Title = $"Samples.PerfCounters | send delay: {sendDelay:N0}ms, process delay: {processDelay:N0}ms, step size: {stepSize:N0}ms, concurrency: {concurrency:N0}, SLA: {sla:N0}s";
                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        if (sendDelay > stepSize) sendDelay -= stepSize;
                        break;
                    case ConsoleKey.UpArrow:
                        sendDelay += stepSize;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (processDelay > stepSize) processDelay -= stepSize;
                        break;
                    case ConsoleKey.RightArrow:
                        processDelay += stepSize;
                        break;
                    case ConsoleKey.OemPlus:
                        if (stepSize < 10000) stepSize *= 10;
                        
                        break;
                    case ConsoleKey.OemMinus:
                        if (stepSize > 10) stepSize /= 10;
                        break;
                    default:
                        return;
                }
            }
        }
        finally
        {
            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }

    static int stepSize = 100;
    public static int processDelay = 100;
    static int sendDelay = 1000;

    static async Task Loop()
    {
        while (true)
        {
            Console.Write("·");
            await endpointInstance.SendLocal(new MyMessage());
            await Task.Delay(sendDelay).ConfigureAwait(false);
        }
    }
}
