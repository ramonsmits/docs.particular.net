﻿using System;
using System.Text;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;
using NLog;
using System.Threading.Tasks;

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

        endpointConfiguration.Recoverability()
            .Immediate(c => c.NumberOfRetries(1))
            .Delayed(c => c.NumberOfRetries(5).TimeIncrease(TimeSpan.FromSeconds(1)));

        endpointConfiguration.DefineCriticalErrorAction(ctx =>
        {
            try
            {
                LogManager.GetLogger("CriticalError").Fatal("CriticalError", ctx.Exception);
                LoggingConfiguration.Teardown();
                return Task.CompletedTask;
            }
            finally
            {
                Environment.FailFast("NServiceBus CriticalError", ctx.Exception);
            }
        });


    }
}
