using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Azure.Transports.WindowsAzureStorageQueues;
using NServiceBus.Features;

class Program
{
    static async Task Main()
    {
        var endpointName = "native-integration-asq";
        Console.Title = endpointName;

        var endpointConfiguration = new EndpointConfiguration(endpointName);
        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();

        var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
        transport.ConnectionString("UseDevelopmentStorage=true");
        transport.DisablePublishing();
        transport.DelayedDelivery().DisableDelayedDelivery();

        #region Native-message-mapping

        transport.UnwrapMessagesWith(message =>
        {
            var wrapper =  new MessageWrapper
            {
                Id = message.MessageId,
                Body = message.Body.ToArray(),
                Headers = new Dictionary<string, string>()
            };

            string enclosedMessageTypes;

            if(message.MessageText.Contains("Property"))
            {
                enclosedMessageTypes = typeof(NativeMessage2).FullName;
            }
            else if(message.MessageText.Contains("Content"))
            {
                enclosedMessageTypes = typeof(NativeMessage).FullName;
            }
            else
            {
                throw new NotSupportedException("Incoming message type could not be resolved");
            }

            Console.WriteLine("Incoming message type is: " + enclosedMessageTypes);

            wrapper.Headers[Headers.EnclosedMessageTypes] = enclosedMessageTypes;

            return wrapper;
        });

        #endregion

        endpointConfiguration.DisableFeature<TimeoutManager>();
        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}
