using Autofac.Extras.Multitenant;

namespace EventSpike.Common
{
    public class ExplicitTenantIdProvider : ITenantIdProvider
    {
        private readonly ITenantIdentificationStrategy _strategy;
        private object _tenantId;

        public void IdentifyAs(object tenantId)
        {
            _tenantId = tenantId;
        }

        public object TenantId
        {
            get { return _tenantId ?? (_strategy.TryIdentifyTenant(out _tenantId) ? _tenantId : null); }
        }

        public ExplicitTenantIdProvider(ITenantIdentificationStrategy strategy)
        {
            _strategy = strategy;
        }
    }
}