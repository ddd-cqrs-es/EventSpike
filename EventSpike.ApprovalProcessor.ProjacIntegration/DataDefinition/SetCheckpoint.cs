namespace EventSpike.ApprovalProcessor.ProjacIntegration.DataDefinition
{
    public class SetCheckpoint
    {
        public readonly string CheckpointToken;
        public readonly string StoreId;

        public SetCheckpoint(string storeId, string checkpointToken)
        {
            StoreId = storeId;
            CheckpointToken = checkpointToken;
        }
    }
}