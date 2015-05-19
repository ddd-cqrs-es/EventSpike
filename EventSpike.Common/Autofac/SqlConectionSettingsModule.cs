using System.Configuration;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;

namespace EventSpike.Common.Autofac
{
    public class SqlConectionSettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConventionTenantSqlConnectionSettingsFactory>().WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId));
            
            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings())
                .InstancePerTenant()
                .AsSelf();

            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings("Projections"))
                .InstancePerTenant()
                .Named<ConnectionStringSettings>("Projections");
        }
    }
}
