using System.Linq;
using Biggy.Core;
using EventSpike.Common.EventSubscription;

namespace EventSpike.Common.Biggy
{
    public class TenantScopedBiggyStoreCheckpointTracker :
        IStoreCheckpointTracker,
        IStoreCheckpointProvider
    {
        private readonly TenantCheckpointTokenDocument _nullTenantCheckpointTokenDocument;

        private readonly BiggyList<TenantCheckpointTokenDocument> _tenantCheckpoints;
        private readonly string _tenantId;

        public TenantScopedBiggyStoreCheckpointTracker(string tenantId, BiggyList<TenantCheckpointTokenDocument> tenantCheckpoints)
        {
            _tenantId = tenantId;
            _tenantCheckpoints = tenantCheckpoints;

            _nullTenantCheckpointTokenDocument = new TenantCheckpointTokenDocument
            {
                TenantId = _tenantId,
                CheckpointToken = NullCheckpointToken.Value
            };
        }

        public string GetCheckpoint()
        {
            var tenantCheckpoint =
                _tenantCheckpoints.SingleOrDefault(x => x.TenantId == _tenantId) ?? _nullTenantCheckpointTokenDocument;

            return tenantCheckpoint.CheckpointToken;
        }

        public void UpdateCheckpoint(string checkpointToken)
        {
            var tenantCheckpoint =
                _tenantCheckpoints.SingleOrDefault(x => x.TenantId == _tenantId);

            if (tenantCheckpoint != null)
            {
                tenantCheckpoint.CheckpointToken = checkpointToken;

                _tenantCheckpoints.Update(tenantCheckpoint);
            }
            else
            {
                _tenantCheckpoints.Add(new TenantCheckpointTokenDocument
                {
                    TenantId = _tenantId,
                    CheckpointToken = checkpointToken
                });
            }
        }
    }
}