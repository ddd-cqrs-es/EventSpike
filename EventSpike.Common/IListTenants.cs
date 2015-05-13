using System.Collections.Generic;

namespace EventSpike.Common
{
    public interface IListTenants
    {
        IEnumerable<string> GetTenantIds();
    }
}
