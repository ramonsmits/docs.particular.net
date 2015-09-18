using NServiceBus;
using NServiceBus.DataBus;

namespace Receiver
{
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration busConfiguration)
        {
            busConfiguration.EndpointName("Samples.AzureBlobStorageDataBus.Receiver");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UseDataBus<AzureDataBus>()
                .ConnectionString("UseDevelopmentStorage=true");
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            busConfiguration.Conventions()
                .DefiningDataBusPropertiesAs(p => p.PropertyType == typeof(byte[]));
        }
    }
}
