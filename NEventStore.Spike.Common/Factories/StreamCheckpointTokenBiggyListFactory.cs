using Biggy.Core;
using Biggy.Data.Json;
using NEventStore.Spike.Common.StreamCheckpointTracker;

namespace NEventStore.Spike.Common.Factories
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
