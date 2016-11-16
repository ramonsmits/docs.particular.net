using System.Globalization;
using NServiceBus;
using NServiceBus.Logging;

public static class LoggingConfiguration
{
    public static void Setup()
    {
        LogManager.Use<NLogFactory>();
        NLog.LogManager.Configuration.DefaultCultureInfo = CultureInfo.InvariantCulture;
    }

    public static void Teardown()
    {
        NLog.LogManager.Shutdown();
    }
}
