using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus.Logging;
using ServiceControl.Plugin.CustomChecks;

abstract class PerformanceCounterCheckBase : CustomCheck
{
    readonly long Threshold;
    readonly string CategoryName;
    readonly string CounterName;
    readonly ILog Log;
    static string HostName = System.Net.Dns.GetHostName();

    protected PerformanceCounterCheckBase(long thresshold, string categoryName, string counterName)
        : base(
            id: $"PerformanceCounter {categoryName}/{counterName}@{HostName}",
            category: "PerformanceCounter",
            repeatAfter: TimeSpan.FromSeconds(5))
    {
        Log = LogManager.GetLogger(GetType());
        Threshold = thresshold;
        CategoryName = categoryName;
        CounterName = counterName;
    }

    public override Task<CheckResult> PerformCheck()
    {
        try
        {
            long value;

            using (var bytes = new PerformanceCounter(CategoryName, CounterName))
            {
                value = bytes.RawValue;
            }

            Log.InfoFormat("Performance counter '{0}/{1}' value {2:N0}", CategoryName, CounterName, value);

            if (value > Threshold)
            {
                var message = $"Performance counter '{CategoryName}/{CounterName}' value {value:N0} exceeded thresshold {Threshold:N0}.";
                Log.Warn(message);
                return CheckResult.Failed(message);
            }

            return CheckResult.Pass;
        }
        catch (Exception exception)
        {
            var error = $"Failed to check Performance counter '{CategoryName}/{CounterName}' on '{HostName}'. Error: {exception.Message}";
            Log.Info(error);
            return CheckResult.Failed(error);
        }
    }
}