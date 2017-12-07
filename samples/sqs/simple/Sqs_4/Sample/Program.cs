using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using NServiceBus;

class Program
{
    const string EndpointName = "Samples.Sqs.Simple";
    const int DefaultConnectionLimit = 150;

    static bool sending = true;
    static bool receiving = true;

    static async Task Main()
    {

        GCSettings.LatencyMode = GCLatencyMode.Batch;
        Console.Title = EndpointName + " | " + Environment.OSVersion + " | Is64BitProcess:" + Environment.Is64BitProcess + " | LatencyMode:" + GCSettings.LatencyMode + " | IsServerGC:" + GCSettings.IsServerGC;

        ServicePointManager.Expect100Continue = false;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.DefaultConnectionLimit = DefaultConnectionLimit; // Querying of DefaultConnectionLimit on dotnet core does not return assigned value

        var endpointConfiguration = new EndpointConfiguration(EndpointName);

        endpointConfiguration.RegisterComponents(x => x.ConfigureComponent<MyHandler>(DependencyLifecycle.SingleInstance));
        var credentials = new EnvironmentVariablesAWSCredentials();
        var region = RegionEndpoint.GetBySystemName("eu-west-2");

        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.Transactions(TransportTransactionMode.None);
        transport.QueueNamePrefix("sqsstability-");
        transport.MaxTimeToLive(TimeSpan.FromMinutes(10));
        transport.ClientFactory(() => new AmazonSQSClient(
            credentials,
            new AmazonSQSConfig
            {
                RegionEndpoint = region,
#if NETCOREAPP2_0
                MaxConnectionsPerServer = DefaultConnectionLimit // dotnet core httpclient ignores DefaultConnectionLimit and uses int.MaxValue
#endif
            }));

        var s3Configuration = transport.S3("ramon-perftest-sqs", "sqsstability-");
        s3Configuration.ClientFactory(() => new AmazonS3Client(
            credentials,
            new AmazonS3Config
            {
                RegionEndpoint = region,
#if NETCOREAPP2_0
                MaxConnectionsPerServer = DefaultConnectionLimit // dotnet core httpclient ignores DefaultConnectionLimit and uses int.MaxValue
#endif
            }));

        endpointConfiguration.LimitMessageProcessingConcurrencyTo(Environment.ProcessorCount * 4);
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        if (!receiving) endpointConfiguration.SendOnly();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        var sendingTask = Task.Run(async () =>
        {
            if (sending)
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMinutes(5));
                var sends = new List<Task>();

                var sessionId = DateTime.UtcNow.ToString("s");
                var batchSize = 100;
                var count = 0;

                while (!cts.Token.IsCancellationRequested)
                {
                    var s = Stopwatch.StartNew();
                    sends.Clear();
                    for (var i = 0; i < batchSize; i++)
                    {
                        var id = $"{sessionId}/{++count:D8}";
                        var options = new SendOptions();
                        options.SetDestination(EndpointName);
                        options.SetMessageId(id);
                        sends.Add(RetryWithBackoff(() => endpointInstance.Send(new MyMessage(), options), cts.Token, id, 5));
                    }
                    await Task.WhenAll(sends).ConfigureAwait(false);
                    var elapsed = s.Elapsed;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Console.Out.WriteLineAsync($"New batch of {batchSize:N0} took {elapsed} at {count:N0} ({batchSize / elapsed.TotalSeconds:N}/s)").ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }).ConfigureAwait(false);

        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
        }

        await sendingTask;

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static Random random = new Random();
    static async Task RetryWithBackoff(Func<Task> action, CancellationToken token, string id, int maxAttempts)
    {
        var attempts = 0;
        while (true)
        {
            try
            {
                await action()
                    .ConfigureAwait(false);
                return;
            }
            catch (Exception ex)
            {
                if (attempts > maxAttempts) throw new InvalidOperationException("Exhausted send retries.", ex);
                double next;
                lock (random) next = random.NextDouble();
                next *= 0.2; // max 20% jitter
                next += 1D;
                next *= 100 * Math.Pow(2, attempts++);
                var delay = TimeSpan.FromMilliseconds(next); // Results in 100ms, 200ms, 400ms, 800ms, etc. including max 20% random jitter.
                await Console.Out.WriteLineAsync($"{id}: #{attempts}").ConfigureAwait(false);
                await Task.Delay(delay, token)
                    .ConfigureAwait(false);
            }
        }
    }
}