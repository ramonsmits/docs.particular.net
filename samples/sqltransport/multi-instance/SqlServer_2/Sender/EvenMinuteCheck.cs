using System;
using ServiceControl.Plugin.CustomChecks;

public class EvenMinuteCheck : PeriodicCheck
{
    public EvenMinuteCheck()
        : base("Even minute are the best", "SomeCategory", TimeSpan.FromSeconds(5))
    {
     }

    public override CheckResult PerformCheck()
    {
        if (DateTime.UtcNow.Minute % 2 == 0)
            return CheckResult.Pass;
        else
            return CheckResult.Failed("Odd minute, that is just so wrong!");
    }
}