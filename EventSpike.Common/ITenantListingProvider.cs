using System.Collections.Generic;

namespace EventSpike.Common
{
    public interface ITenantListingProvider
    {
        IEnumerable<string> GetTenantIds();
    }
}
