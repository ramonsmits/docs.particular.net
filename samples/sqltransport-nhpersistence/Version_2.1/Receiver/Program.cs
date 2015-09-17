using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Tool.hbm2ddl;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBus.Transports.SQLServer;

class Program
{
    public static CountdownEvent X;

    static void Main()
    {

        BusConfiguration busConfiguration = new BusConfiguration();

        Customize(busConfiguration);

        //.RegisterManagedSessionInTheContainer();

        busConfiguration.PurgeOnStartup(true);
        busConfiguration.EnableInstallers();

        X = new CountdownEvent(256);

        long orderId = 0;

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine(
                "IsServerGC:{0} LatencyMode:{1} Is64BitProcess:{2}",
                System.Runtime.GCSettings.IsServerGC,
                System.Runtime.GCSettings.LatencyMode,
                System.Environment.Is64BitProcess);

            Console.ReadKey();
            Console.WriteLine("Press CTRL+C key to exit");
            var start = Stopwatch.StartNew();
            var interval = Stopwatch.StartNew();
            while (true)
            {
                X.Reset();

                Console.Write("*");
                Parallel.For(0, X.InitialCount, i =>
                {
                    var id = Interlocked.Increment(ref orderId);
                    bus.SendLocal(new SubmitOrder
                    {
                        OrderId = id.ToString(CultureInfo.InvariantCulture)
                    });
                });

                X.Wait();

                var elapsedTicks = Interval(interval);

                var perMessage = elapsedTicks / X.InitialCount;

                var currentThroughput = TimeSpan.TicksPerSecond / perMessage;
                var averageThroughput = TimeSpan.TicksPerSecond / (start.ElapsedTicks / orderId);
                Console.Title = string.Format("{0:N0}/s ~{1:N0}/s +{2:N0} @{3:N0}s", currentThroughput, averageThroughput, orderId, start.Elapsed.TotalSeconds);

                Thread.Sleep(15000); // Should result in downscale of polling sql server threads
            }
        }
    }

    static long Interval(Stopwatch sw)
    {
        var elapsed = sw.ElapsedTicks;
        sw.Restart();
        return elapsed;
    }

    static void Customize(BusConfiguration configuration)
    {
        var sqlServerTimeout = TimeSpan.FromSeconds(120);

        configuration.UseTransport<SqlServerTransport>()
            .DefaultSchema("workflow")
            //.PauseAfterReceiveFailure(sqlServerTimeout)
            .TimeToWaitBeforeTriggeringCircuitBreaker(sqlServerTimeout);
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.UsePersistence<NHibernatePersistence, StorageType.Timeouts>()
            .RegisterManagedSessionInTheContainer();

        configuration.TimeToWaitBeforeTriggeringCriticalErrorOnTimeoutOutages(sqlServerTimeout);
        //configuration.EndpointName(endpointName);
        configuration.UseSerialization<JsonSerializer>();
        configuration.EnableInstallers();
    }
}
