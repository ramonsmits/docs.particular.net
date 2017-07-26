using NServiceBus;
using NServiceBus.Features;

public static class EndpointConfigurationExtensions
{
    public static TransportExtensions<AzureStorageQueueTransport> ApplyGlobalSettings(this EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.DisableFeature<AutoSubscribe>();
        endpointConfiguration.UseSerialization<JsonSerializer>();
        var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>()
            .ConnectionString("UseDevelopmentStorage=true");

        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");

        return transport;
    }
}