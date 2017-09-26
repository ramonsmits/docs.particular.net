using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NServiceBus;
using NServiceBus.Logging;
using Store.Messages.Commands;
using Store.Messages.Events;

public class MvcApplication :
    HttpApplication
{
    public static IEndpointInstance EndpointInstance;
    static LoadSimulator LoadSimulator;

    protected void Application_End()
    {
        AsyncStop().GetAwaiter().GetResult();
    }

    protected void Application_Start()
    {
        AsyncStart().GetAwaiter().GetResult();
    }

    static async Task AsyncStop()
    {
        await LoadSimulator.Stop()
            .ConfigureAwait(false);
        await EndpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static async Task AsyncStart()
    {
        LoggingConfiguration.Setup();

        var endpointConfiguration = new EndpointConfiguration("Store.ECommerce");
        endpointConfiguration.PurgeOnStartup(true);
        endpointConfiguration.ApplyCommonConfiguration(transport =>
        {
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(SubmitOrder).Assembly, "Store.Messages.Commands", "Store.Sales");
            routing.RegisterPublisher(typeof(SubmitOrder).Assembly, "Store.Messages.Events", "Store.Sales");
            routing.RegisterPublisher(typeof(DownloadIsReady), "Store.ContentManagement");
        });

        EndpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        AreaRegistration.RegisterAllAreas();
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);

        var orderNumber = int.MaxValue / 2;

        var productIds = new[]
        {
            "videos",
            "training",
            "documentation",
            "customers",
            "platform"
        };

        var random = new Random();
        LoadSimulator = new LoadSimulator(TimeSpan.Zero, TimeSpan.FromSeconds(1), () =>
            EndpointInstance.Send(new SubmitOrder
            {
                ClientId = random.Next(1000).ToString(),
                OrderNumber = Interlocked.Increment(ref orderNumber),
                ProductIds = productIds.Take(random.Next(productIds.Length)).ToArray(),
                CreditCardNumber = "1234 5678 9012 2345",
                ExpirationDate = "10/13"
            })
        );
        await LoadSimulator.Start()
            .ConfigureAwait(false);
    }
}
