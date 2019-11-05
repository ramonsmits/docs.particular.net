#define FAST

extern alias fast;
extern alias slow;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        ServicePointManager.Expect100Continue = false;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.DefaultConnectionLimit = 150;
        Console.Title = "Samples.ASBS.SendReply.Endpoint1";

        #region config

        var endpointConfiguration = new EndpointConfiguration("Samples.ASBS.SendReply.Endpoint1");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.EnableMetrics().SendMetricDataToServiceControl("Particular.ServiceControl.ASB.Monitoring", TimeSpan.FromSeconds(2));
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(Environment.ProcessorCount * 4);
        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }
        endpointConfiguration.UseSerialization<XmlSerializer>();

#if FAST
        var numberOfFactoriesAndClients = Math.Min(64, Environment.ProcessorCount * 4);
        var transport = endpointConfiguration.UseTransport<fast.NServiceBus.AzureServiceBusTransport>();
        // ReSharper disable RedundantNameQualifier
        fast.NServiceBus.AzureServiceBusTransportExtensions.UseForwardingTopology(transport);
        //fast.NServiceBus.AzureServiceBusTransportExtensions.MessagingFactories(transport).BatchFlushInterval(TimeSpan.FromMilliseconds(100));
        //fast.NServiceBus.AzureServiceBusTransportExtensions.MessagingFactories(transport).NumberOfMessagingFactoriesPerNamespace(numberOfFactoriesAndClients);
        //fast.NServiceBus.AzureServiceBusTransportExtensions.NumberOfClientsPerEntity(transport, numberOfFactoriesAndClients);
        //fast.NServiceBus.AzureServiceBusTransportExtensions.Queues(transport).EnablePartitioning(true);
        //fast.NServiceBus.AzureServiceBusTransportExtensions.Topics(transport).EnablePartitioning(true);
        // ReSharper enable RedundantNameQualifier
#else
        var transport = endpointConfiguration.UseTransport<slow.NServiceBus.AzureServiceBusTransport>();
        //transport.UseWebSockets();
        //transport.EnablePartitioning();
#endif
        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        transport.ConnectionString(connectionString);

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press 'enter' to send a message");
        Console.WriteLine("Press any other key to exit");


        var message = new Message1
        {
            Property = "Hello from Endpoint1"
        };



        while (true)
        {
            await sendsLimiter.WaitAsync().ConfigureAwait(false);
            Send(endpointInstance, "Samples.ASBS.SendReply.Endpoint2", message);
        }

        //await endpointInstance.Stop().ConfigureAwait(false);
    }

    static SemaphoreSlim sendsLimiter = new SemaphoreSlim(1024);

    static async void Send(IEndpointInstance context, string destination, object message)
    {
        try
        {
            await context.Send(destination, message).ConfigureAwait(false);
        }
        finally
        {
            //Console.Write("<");
            sendsLimiter.Release();
        }
    }
}
