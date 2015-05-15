using System.Collections.Generic;
using Autofac;
using Autofac.Extras.Multitenant;

namespace EventSpike.Common.Autofac
{
    public class TenantModlue : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MultitenantContainer>()
                .SingleInstance()
                .Named<ILifetimeScope>(MassTransitModule.InstanceNames.LifetimeScope);

            builder.Register(context => context.Resolve<IListTenants>().GetTenantIds())
                .Named<IEnumerable<string>>(InstanceNames.AllTenantIds);

            builder.RegisterType<MassTransitContextTenantIdentificationStrategy>()
                .As<ITenantIdentificationStrategy>()
                .SingleInstance();

            builder.Register(context =>
            {
                object tenantId;
                context.Resolve<ITenantIdentificationStrategy>().TryIdentifyTenant(out tenantId);
                return tenantId != null ? tenantId.ToString() : null;
            }).Named<string>(InstanceNames.CurrentTenantId);
        }
    }
}