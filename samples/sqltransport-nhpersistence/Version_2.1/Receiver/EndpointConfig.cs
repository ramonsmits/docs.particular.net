
namespace Receiver
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Customize()
        {
        }

        public void Init()
        {
            var configuration = Configure.With();

            configuration.PurgeOnStartup(true);

            var endpointName = "memleaktest";
            configuration.DefineEndpointName(endpointName);
            
            // IStartableBus startableBus = configuration.CreateBus();
            // startableBus.Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());
        }
    }
}
