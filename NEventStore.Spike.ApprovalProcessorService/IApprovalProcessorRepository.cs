namespace NEventStore.Spike.ApprovalProcessorService
{
    internal interface IApprovalProcessorRepository
    {
        ApprovalProcessor GetProcessorById(string id);
    }
}