using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extras.Multitenant;

namespace EventSpike.Common.Autofac
{
    public class TenantModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MultitenantContainer>()
                .SingleInstance()
                .As<MultitenantContainer>()
                .Named<ILifetimeScope>(MassTransitModule.MassTransitInstanceNames.LifetimeScope);

            builder.Register(context => context.Resolve<IListTenants>().GetTenantIds())
                .Named<IEnumerable<string>>(InstanceNames.AllTenantIds);

            builder.RegisterType<MassTransitMessageHeadersTenantIdentificationProvider>()
                .As<ITenantIdentificationStrategy>();

            builder.Register(context =>
            {
                object tenantId;
                context.Resolve<ITenantIdentificationStrategy>().TryIdentifyTenant(out tenantId);
                return tenantId != null ? tenantId.ToString() : null;
            })
            .InstancePerTenant()
            .Named<string>(InstanceNames.CurrentTenantId);

            builder.RegisterType<SystemInitializer>()
                .As<ISystemInitializer>()
                .WithParameter((param, context) => param.ParameterType == typeof (IEnumerable<INeedInitialization>), (param, context) =>
                {
                    var tenantIds = context.ResolveNamed<IEnumerable<string>>(InstanceNames.AllTenantIds);
                    var multitenantContainer = context.Resolve<MultitenantContainer>();

                    return tenantIds.Select(tenantId => multitenantContainer.GetTenantScope(tenantId).Resolve<IEnumerable<INeedInitialization>>().ToList()).SelectMany(_ => _).Distinct();
                });
        }
    }
}