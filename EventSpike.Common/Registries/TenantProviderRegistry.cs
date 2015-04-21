using System;
using System.Collections.Concurrent;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class TenantProviderRegistry : Registry
    {
        private readonly Action<ConfigurationExpression> _tenantContainerConfiguration;
        private readonly ConcurrentDictionary<string, IContainer> _tenantContainers = new ConcurrentDictionary<string, IContainer>();

        public TenantProviderRegistry(Action<ConfigurationExpression> tenantContainerConfiguration)
        {
            _tenantContainerConfiguration = tenantContainerConfiguration;

            For(typeof (ITenantProvider<>))
                .Use(typeof (StructureMapTenantContainerProvider<>));

            For<IContainer>()
                .MissingNamedInstanceIs
                .ConstructedBy(context => _tenantContainers.GetOrAdd(context.RequestedName, _ => CreateTenantContainer(context)));
        }

        private IContainer CreateTenantContainer(IContext context)
        {
            var childContainer = context.GetInstance<IContainer>()
                .CreateChildContainer();

            childContainer.Configure(configure =>
            {
                configure
                    .For<string>()
                    .Add(context.RequestedName)
                    .Named("tenantId");

                _tenantContainerConfiguration(configure);
            });

            return childContainer;
        }
    }
}