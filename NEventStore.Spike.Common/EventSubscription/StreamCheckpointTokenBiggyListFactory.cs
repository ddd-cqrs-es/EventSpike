using Biggy.Core;
using Biggy.Data.Json;

namespace NEventStore.Spike.Common.EventSubscription
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