using Biggy.Core;
using Biggy.Data.Json;

namespace EventSpike.Common.Biggy
{
    public class StreamCheckpointTokenBiggyListFactory
    {
        public BiggyList<TenantCheckpointTokenDocument> Construct()
        {
            var store = new JsonStore<TenantCheckpointTokenDocument>();

            return new BiggyList<TenantCheckpointTokenDocument>(store);
        }
    }
}