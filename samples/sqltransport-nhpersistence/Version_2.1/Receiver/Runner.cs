using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Config;
using Receiver;

public class Runner : IWantToRunWhenBusStartsAndStops
{
    public static CountdownEvent X;

    private IBus bus { get { return BusInstance.busInstance; } }

    public void Start()
    {
        X = new CountdownEvent(256);

        long orderId = 0;

        Console.WriteLine("IsServerGC:{0} ({1})", System.Runtime.GCSettings.IsServerGC, System.Runtime.GCSettings.LatencyMode);
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

            var currentThroughput = TimeSpan.TicksPerHour / perMessage;
            var averageThroughput = TimeSpan.TicksPerHour / (start.ElapsedTicks / orderId);
            Console.Title = string.Format("{0:N0}/h ~{1:N0}/h +{2:N0} @{3:N0}s", currentThroughput, averageThroughput, orderId, start.Elapsed.TotalSeconds);

            //Thread.Sleep(15000); // Should result in downscale of polling sql server threads
        }
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    static long Interval(Stopwatch sw)
    {
        var elapsed = sw.ElapsedTicks;
        sw.Restart();
        return elapsed;
    }

    //static void Customize(BusConfiguration configuration)
    //{
    //    var sqlServerTimeout = TimeSpan.FromSeconds(120);

    //    configuration.UseTransport<SqlServerTransport>()
    //        .DefaultSchema("workflow")
    //        //.PauseAfterReceiveFailure(sqlServerTimeout)
    //        .TimeToWaitBeforeTriggeringCircuitBreaker(sqlServerTimeout);
    //    configuration.UsePersistence<InMemoryPersistence>();
    //    configuration.UsePersistence<NHibernatePersistence, StorageType.Timeouts>()
    //        .RegisterManagedSessionInTheContainer();

    //    configuration.TimeToWaitBeforeTriggeringCriticalErrorOnTimeoutOutages(sqlServerTimeout);
    //    //configuration.EndpointName(endpointName);
    //    configuration.UseSerialization<JsonSerializer>();
    //    configuration.EnableInstallers();
    //}
}
