using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        var queueClient = new QueueClient("UseDevelopmentStorage=true", "native-integration-asq");

        await queueClient.CreateIfNotExistsAsync().ConfigureAwait(false);

        Console.WriteLine("Press Enter to send a native message.");
        Console.WriteLine("Press any other key to exit.");
        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }

            #region send-a-native-message

            var nativeMessage = new NativeMessage
            {
                Content = $"Hello from native sender @ {DateTimeOffset.Now}"
            };

            var serializedMessage = JsonConvert.SerializeObject(nativeMessage);

            await queueClient.SendMessageAsync(serializedMessage).ConfigureAwait(false);

            #endregion

            Console.WriteLine("NativeMessage sent");

            var msg2 = new NativeMessage2
            {
                Property = $"Hello from native sender @ {DateTimeOffset.Now}"
            };

            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(msg2)).ConfigureAwait(false);

            Console.WriteLine("NativeMessage2 sent");
        }
    }
}
