using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using Receiver;
using NServiceBus.Logging;

public class Runner : IWantToRunWhenBusStartsAndStops
{
    ILog Log = LogManager.GetLogger(typeof(Runner));
    public static CountdownEvent X;
    public IBus Bus { get; set; }
    private Task loopTask;
    private bool shutdown;
    private System.Threading.CancellationTokenSource stopLoop;

    public void Start()
    {
        Log.Info("Starting...");
        stopLoop = new CancellationTokenSource();
        loopTask = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
        Log.Info("Started");
    }

    void Loop(object o)
    {
        X = new CountdownEvent(256);

        long orderId = 0;

        Log.InfoFormat("IsServerGC:{0} ({1})", System.Runtime.GCSettings.IsServerGC, System.Runtime.GCSettings.LatencyMode);
        Log.InfoFormat("ProcessorCount: {0}", Environment.ProcessorCount);
        Log.InfoFormat("64bit: {0}", Environment.Is64BitProcess);

        Console.WriteLine("Press CTRL+C key to exit");
        var start = Stopwatch.StartNew();
        var interval = Stopwatch.StartNew();
        while (!shutdown)
        {
            X.Reset();

            Console.Write("*");
            Parallel.For(0, X.InitialCount, i =>
            {
                var id = Interlocked.Increment(ref orderId);
                Bus.SendLocal(new SubmitOrder
                {
                    OrderId = id.ToString(CultureInfo.InvariantCulture)
                });
            });

            try
            {
                X.Wait(stopLoop.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            var elapsedTicks = Interval(interval);

            var perMessage = elapsedTicks / X.InitialCount;

            var currentThroughput = TimeSpan.TicksPerHour / perMessage;
            var averageThroughput = TimeSpan.TicksPerHour / (start.ElapsedTicks / orderId);
            Console.Title = string.Format("{0:N0}/h ~{1:N0}/h +{2:N0} @{3:N0}s p{4:N0}", currentThroughput, averageThroughput, orderId, start.Elapsed.TotalSeconds, processedCount);

            //Thread.Sleep(15000); // Should result in downscale of polling sql server threads
        }
    }

    public void Stop()
    {
        Log.Info("Stopping...");
        shutdown = true;
        using (stopLoop)
        {
            stopLoop.Cancel();
            using (loopTask)
            {
                loopTask.Wait();
            }
        }

        Log.Info("Stopped");
    }

    static long Interval(Stopwatch sw)
    {
        var elapsed = sw.ElapsedTicks;
        sw.Restart();
        return elapsed;
    }

    private static long processedCount;

    internal static void Signal()
    {
        Interlocked.Increment(ref processedCount);
        X.Signal();
    }
}
