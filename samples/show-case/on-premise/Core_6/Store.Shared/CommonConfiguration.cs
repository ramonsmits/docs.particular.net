using System;
using System.Text;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;

public static class CommonConfiguration
{
    public static void ApplyCommonConfiguration(
        this EndpointConfiguration endpointConfiguration,
        Action<TransportExtensions<MsmqTransport>> messageEndpointMappings = null
        )
    {
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        messageEndpointMappings?.Invoke(transport);

        var persistence = endpointConfiguration.UsePersistence<NHibernatePersistence>();
        //persistence.ConnectionString(@"Data Source=.\SQLEXPRESS;Integrated Security=True;Database=nservicebus");

        var defaultKey = "2015-10";
        var ascii = Encoding.ASCII;
        var encryptionService = new RijndaelEncryptionService(
            encryptionKeyIdentifier: defaultKey,
            key: ascii.GetBytes("gdDbqRpqdRbTs3mhdZh9qCaDaxJXl+e6"));
        endpointConfiguration.EnableMessagePropertyEncryption(encryptionService);

        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.CustomCheckPlugin("particular.servicecontrol");
    }
}
