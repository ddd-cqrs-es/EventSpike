using System.Linq;
using Biggy.Core;
using Biggy.Data.Json;

namespace NEventStore.Spike.Common.CheckpointTracker
{
    public class BiggyFileTenantCheckpointTracker :
        IStreamCheckpointTracker
    {
        private static readonly TenantCheckpointToken NullTenantCheckpointToken = new TenantCheckpointToken();

        private readonly BiggyList<TenantCheckpointToken> _tenantCheckpoints;

        public BiggyFileTenantCheckpointTracker(string fileName)
        {
            var store = new JsonStore<TenantCheckpointToken>(fileName);

            _tenantCheckpoints = new BiggyList<TenantCheckpointToken>(store);
        }

        public string GetLastCheckpoint()
        {
            var tenantCheckpoint = _tenantCheckpoints.SingleOrDefault() ?? NullTenantCheckpointToken;

            return tenantCheckpoint.CheckpointToken;
        }

        public void UpdateCheckpoint(string checkpointToken)
        {
            var tenantCheckpoint = _tenantCheckpoints.SingleOrDefault();

            if (tenantCheckpoint != null)
            {
                tenantCheckpoint.CheckpointToken = checkpointToken;

                _tenantCheckpoints.Update(tenantCheckpoint);
            }
            else
            {
                _tenantCheckpoints.Add(new TenantCheckpointToken
                {
                    CheckpointToken = checkpointToken
                });
            }
        }
    }
}