using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;
using EventSpike.Runtime;

namespace EventSpike.Checkpointing.Biggy
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
                .As<IProvideStoreCheckpoints>()
                .InstancePerTenant();

            builder.RegisterType<BiggyTenantListingProvider>().SingleInstance().As<IListTenants>();
        }
    }
}