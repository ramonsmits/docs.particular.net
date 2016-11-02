using NServiceBus.Logging;

public class MyService
{
    static ILog log = LogManager.GetLogger<MyService>();
    
    public void WriteHello()
    {
        log.Warn("Hello from MyService.");
    }
}