using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Runtime;

namespace EventSpike.Checkpointing.Biggy
{
    public class BiggyTenantListingProvider :
        IListTenants
    {
        private readonly BiggyList<TenantCheckpointTokenDocument> _tenantCheckpoints;

        public BiggyTenantListingProvider(BiggyList<TenantCheckpointTokenDocument> tenantCheckpoints)
        {
            _tenantCheckpoints = tenantCheckpoints;
        }

        public IEnumerable<string> GetTenantIds()
        {
            return _tenantCheckpoints.Select(x => x.TenantId);
        }
    }
}