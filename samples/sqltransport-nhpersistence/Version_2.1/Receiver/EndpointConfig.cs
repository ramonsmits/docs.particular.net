
using System;
using System.Net.Mime;
using NHibernate.Cfg;
using NServiceBus.Persistence;

namespace Receiver
{
    using NServiceBus;

    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize()
        {
            var configuration = Configure.With();

            configuration
                .PurgeOnStartup(true);
            configuration
                .UseTransport<SqlServer>();
            configuration.InMemorySubscriptionStorage();
            configuration.InMemoryFaultManagement();
            configuration.UseNHibernateTimeoutPersister();

            var endpointName = "memleaktest";
            configuration.DefineEndpointName(endpointName);
            BusInstance.EndpointName = endpointName;

            Configure.Serialization.Json();
        }
    }

    public class BusInstance : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }
        internal static IBus busInstance = null;
        private static readonly object lockObj = new object();
        public static string EndpointName = string.Empty;

        public void Start()
        {
            lock (lockObj)
            {
                if (busInstance != null) throw new Exception("The bus was already started.");
                busInstance = Bus;
            }
        }

        public void Stop()
        {
            lock (lockObj)
            {
                busInstance = null;
            }
        }

        public static void Send(object command)
        {
            lock (lockObj)
            {
                if (busInstance == null) throw new Exception("Bus is not started.");
                busInstance.Send(command);
            }
        }
    }
}
