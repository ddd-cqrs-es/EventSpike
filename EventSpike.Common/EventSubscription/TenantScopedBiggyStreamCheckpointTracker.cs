using System.Linq;
using Biggy.Core;

namespace EventSpike.Common.EventSubscription
{
    public class TenantScopedBiggyStreamCheckpointTracker :
        IStreamCheckpointTracker
    {
        private readonly TenantCheckpointTokenDocument _nullTenantCheckpointTokenDocument;

        private readonly BiggyList<TenantCheckpointTokenDocument> _tenantCheckpoints;
        private readonly string _tenantId;

        public TenantScopedBiggyStreamCheckpointTracker(string tenantId,
            BiggyList<TenantCheckpointTokenDocument> tenantCheckpoints)
        {
            _tenantId = tenantId;
            _tenantCheckpoints = tenantCheckpoints;

            _nullTenantCheckpointTokenDocument = new TenantCheckpointTokenDocument
            {
                TenantId = _tenantId,
                CheckpointToken = NullCheckpointToken.Value
            };
        }

        public string GetLastCheckpoint()
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