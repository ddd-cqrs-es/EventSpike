using System;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public interface IApprovalProcessorRepository
    {
        ApprovalProcessorInstance GetProcessorById(Guid id);
    }
}