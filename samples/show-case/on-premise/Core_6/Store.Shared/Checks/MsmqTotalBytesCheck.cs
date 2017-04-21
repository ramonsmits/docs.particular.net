class MsmqTotalBytesCheck : PerformanceCounterCheckBase
{
    public MsmqTotalBytesCheck() : base(1024 * 1024 * 128, "MSMQ Service", "Total bytes in all queues") // 128MB
    {
    }
}