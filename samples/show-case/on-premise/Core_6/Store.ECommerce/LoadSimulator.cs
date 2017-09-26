using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Logging;

// Simulates busy (almost no delay) / quite time in a sine wave
class LoadSimulator
{
    ILog log = LogManager.GetLogger("LoadSimulator");
    CancellationTokenSource tokenSource = new CancellationTokenSource();
    TimeSpan minimumDelay;
    TimeSpan idleDuration;
    Task fork;
    Func<Task> work;

    public LoadSimulator(TimeSpan minimumDelay, TimeSpan idleDuration, Func<Task> work)
    {
        this.minimumDelay = minimumDelay;
        this.idleDuration = TimeSpan.FromTicks(idleDuration.Ticks / 2);
        this.work = work;
    }

    public Task Start()
    {
        fork = Loop();
        return Task.CompletedTask;
    }

    async Task Loop()
    {
        try
        {
            while (true)
            {
                try
                {
                    log.Info("Invoking task");
                    await work()
                        .ConfigureAwait(false);
                    log.Info("Task completed");
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    log.Error("Task failed", ex);
                }
                var delay = NextDelay();
                log.InfoFormat("Next invocation at {0}", delay);
                await Task.Delay(delay, tokenSource.Token)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    int index;

    TimeSpan NextDelay()
    {
        var angleInRadians = Math.PI / 180.0 * ++index;
        var delay = TimeSpan.FromMilliseconds(idleDuration.TotalMilliseconds * Math.Sin(angleInRadians));
        delay += idleDuration;
        delay += minimumDelay;
        return delay;
    }

    public Task Stop()
    {
        tokenSource.Cancel();
        return fork;
    }
}