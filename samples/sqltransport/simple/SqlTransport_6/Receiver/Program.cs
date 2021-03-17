using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        await Console.Out.WriteLineAsync($"PerformanceCounterCategory.Exists: {PerformanceCounterCategory.Exists("NServiceBus")}");
        await Console.Out.WriteLineAsync($"PerformanceCounterCategory.CounterExists: {PerformanceCounterCategory.CounterExists("SLA violation countdown", "NServiceBus")}");

        await Console.Out.WriteLineAsync($"Environment.Version: {Environment.Version}");
        await Console.Out.WriteLineAsync($"Environment.OSVersion: {Environment.OSVersion}");
        await Console.Out.WriteLineAsync($"RuntimeInformation.FrameworkDescription: {RuntimeInformation.FrameworkDescription}");

        var endpointConfiguration = new EndpointConfiguration("Samples.SqlServer.SimpleReceiver");

        var performanceCounters = endpointConfiguration.EnableWindowsPerformanceCounters();
        performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(100));

        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        var connection = @"Data Source=.;Database=SqlServerSimple;Integrated Security=True;Max Pool Size=100";
        transport.ConnectionString(connection);
        transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

        endpointConfiguration.EnableInstallers();

        SqlHelper.EnsureDatabaseExists(connection);
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.WriteLine("Waiting for message from the Sender");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
