using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extras.Multitenant;
using Magnum.Extensions;

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

            builder.Register(context => context.ResolveOptional<IListTenants>().With(lister => lister.GetTenantIds()) ?? Enumerable.Empty<string>())
                .Named<IEnumerable<string>>(InstanceNames.AllTenantIds);

            builder.RegisterType<MassTransitMessageHeadersTenantIdentificationProvider>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .As<ITenantIdentificationStrategy>();
            
            builder.RegisterType<ExplicitTenantIdProvider>().AsSelf().As<ITenantIdProvider>().InstancePerTenant();

            builder.Register(context => context.Resolve<ITenantIdProvider>().TenantId.With(_ => _.ToString()) ?? Constants.DefaultTenantId)
                .InstancePerTenant()
                .Named<string>(InstanceNames.CurrentTenantId);

            builder.RegisterType<SystemInitializer>()
                .As<ISystemInitializer>()
                .WithParameter((param, context) => param.ParameterType == typeof (IEnumerable<INeedInitialization>), (param, context) =>
                {
                    var tenantIds = context.ResolveNamed<IEnumerable<string>>(InstanceNames.AllTenantIds);
                    var multitenantContainer = context.Resolve<MultitenantContainer>();

                    return tenantIds.Select(tenantId =>
                    {
                        var scope = multitenantContainer.GetTenantScope(tenantId);
                        scope.Resolve<ExplicitTenantIdProvider>().IdentifyAs(tenantId);
                        return scope.Resolve<IEnumerable<INeedInitialization>>().ToList();
                    }).SelectMany(_ => _).Distinct();
                });
        }
    }
}