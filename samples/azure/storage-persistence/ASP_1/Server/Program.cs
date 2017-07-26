using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.Azure.StoragePersistence.Server";

        var endpointConfiguration = new EndpointConfiguration("Samples.Azure.StoragePersistence.Server");
        endpointConfiguration.ApplyGlobalSettings();

        var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
        persistence.ConnectionString("UseDevelopmentStorage=true");
        endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Sagas>()
            .AssumeSecondaryIndicesExist();

        endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Subscriptions>()
            //.TableName("subscriptions6")
            ;

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}