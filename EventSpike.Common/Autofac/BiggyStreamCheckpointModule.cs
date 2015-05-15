using Autofac;
using Autofac.Core;
using EventSpike.Common.Biggy;
using EventSpike.Common.EventSubscription;

namespace EventSpike.Common.Autofac
{
    public class BiggyStreamCheckpointModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new StreamCheckpointTokenBiggyListFactory().Construct())
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<TenantScopedBiggyStoreCheckpointTracker>()
                .WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId))
                .As<ITrackStoreCheckpoints>()
                .As<IProvideStoreCheckpoints>();

            builder.RegisterType<BiggyTenantListingProvider>().SingleInstance().As<IListTenants>();
        }
    }
}