class MsmqTotalMessagesCheck : PerformanceCounterCheckBase
{
    public MsmqTotalMessagesCheck() : base(100000, "MSMQ Service", "Total messages in all queues")
    {
    }
}