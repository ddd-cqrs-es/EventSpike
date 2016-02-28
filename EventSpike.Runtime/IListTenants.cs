using System.Collections.Generic;

namespace EventSpike.Runtime
{
    public interface IListTenants
    {
        IEnumerable<string> GetTenantIds();
    }
}
