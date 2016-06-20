using System;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

class ProvideSecondLevelRetriesConfig : IProvideConfiguration<SecondLevelRetriesConfig>
{
    public SecondLevelRetriesConfig GetConfiguration()
    {
        return new SecondLevelRetriesConfig
        {
            Enabled = false,
            NumberOfRetries = 2,
            TimeIncrease = TimeSpan.FromSeconds(10)
        };
    }
}