using NServiceBus;
using NServiceBus.Features;

public static class BusConfigurationExceptions
{
    public static void ApplyGlobalSettings(this BusConfiguration busConfiguration)
    {
        busConfiguration.DisableFeature<AutoSubscribe>();
        busConfiguration.UseTransport<AzureStorageQueueTransport>().ConnectionString("UseDevelopmentStorage=true");
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EnableInstallers();
    }
}
