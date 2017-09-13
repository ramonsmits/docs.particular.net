using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using ServiceControl.Plugin.CustomChecks;

namespace Store.Shared.SelfTest
{
    class SelfTestFeature : Feature
    {
        public SelfTestFeature()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            context.RegisterStartupTask(new MyStartupTask());
        }
    }

    class MyStartupTask : FeatureStartupTask
    {
        Task loop;
        bool stop;
        protected override Task OnStart(IMessageSession session)
        {
            loop = Loop(session);
            return Task.CompletedTask;
        }

        async Task Loop(IMessageSession session)
        {
            while(!stop)
            {
                await session.SendLocal(new Ping()).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            }
        }
        protected override Task OnStop(IMessageSession session)
        {
            stop = true;
            return loop;
        }

    }

    class Ping : IMessage
    {
    }

    class PingHandler : IHandleMessages<Ping>
    {
        static readonly ILog Log = LogManager.GetLogger(nameof(SelfTestFeature));

        public Task Handle(Ping message, IMessageHandlerContext context)
        {
            Log.Info("Ping to self processed");
            return Task.CompletedTask;
        }
    }
}
