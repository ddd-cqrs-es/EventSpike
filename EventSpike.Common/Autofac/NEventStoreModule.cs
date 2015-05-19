using System.Configuration;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;
using NEventStore;

namespace EventSpike.Common.Autofac
{
    public class NEventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NEventStoreFactory>().AsSelf();

            builder.Register(context => context.Resolve<NEventStoreFactory>().Create()).As<IStoreEvents>().InstancePerTenant();

            builder.RegisterType<NEventStoreFactory>()
                .WithParameter(ResolvedParameter.ForNamed<ConnectionStringSettings>("Projections"))
                .Named<NEventStoreFactory>("Projections")
                .InstancePerTenant();

            builder.Register(context => context.ResolveNamed<NEventStoreFactory>("Projections").Create())
                .Named<IStoreEvents>("Projections")
                .InstancePerTenant();
        }
    }
}