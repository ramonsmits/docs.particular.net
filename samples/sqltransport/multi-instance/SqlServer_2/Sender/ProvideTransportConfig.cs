using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

class ProvideTransportConfig : IProvideConfiguration<TransportConfig>
{
    public TransportConfig GetConfiguration()
    {
        return new TransportConfig
        {
            MaxRetries = 0,
            MaximumConcurrencyLevel = 50
        };
    }
}