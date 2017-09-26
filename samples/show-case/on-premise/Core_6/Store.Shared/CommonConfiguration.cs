using System;
using System.IO;
using System.Net;
using System.Text;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;
using NLog;
using System.Threading.Tasks;
using NServiceBus.Persistence;

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
        persistence.ConnectionString(@"Data Source=.;Integrated Security=True;Database=nservicebus");

        var defaultKey = "2015-10";
        var ascii = Encoding.ASCII;
        var encryptionService = new RijndaelEncryptionService(
            encryptionKeyIdentifier: defaultKey,
            key: ascii.GetBytes("gdDbqRpqdRbTs3mhdZh9qCaDaxJXl+e6"));
        endpointConfiguration.EnableMessagePropertyEncryption(encryptionService);

        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.CustomCheckPlugin("particular.servicecontrol");

        endpointConfiguration.Recoverability()
            .Immediate(c => c.NumberOfRetries(1))
            .Delayed(c => c.NumberOfRetries(5).TimeIncrease(TimeSpan.FromSeconds(1)));

        endpointConfiguration.DefineCriticalErrorAction(ctx =>
        {
            try
            {
                LogManager.GetLogger("CriticalError").Fatal(ctx.Exception, "CriticalError");
                LoggingConfiguration.Teardown();
                return Task.CompletedTask;
            }
            finally
            {
                Environment.FailFast("NServiceBus CriticalError", ctx.Exception);
            }
        });

        if (System.Diagnostics.Debugger.IsAttached) endpointConfiguration.EnableInstallers();

        transport.ApplyLabelToMessages(headers => (headers.ContainsKey(Headers.EnclosedMessageTypes) ? headers[Headers.EnclosedMessageTypes].Substring(0, Math.Min(200, headers[Headers.EnclosedMessageTypes].Length)) + "@" : string.Empty) + DateTime.UtcNow.ToString("O"));


        var instanceId = Dns.GetHostName() + "-" + Lock.Instance;

#pragma warning disable CS0618 // Type or member is obsolete
        endpointConfiguration.EnableMetrics().SendMetricDataToServiceControl("Particular.ServiceControl.Monitoring", TimeSpan.FromSeconds(10), instanceId: instanceId);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    static readonly FileInstanceLock Lock = new FileInstanceLock();
}


class FileInstanceLock
{
    FileStream f;
    public int Instance { get; }

    public FileInstanceLock()
    {
        do
        {
            try
            {
                var lockFileName = ".~" + ++Instance + ".lock";
                f = File.Open(lockFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        } while (f == null);
    }
}