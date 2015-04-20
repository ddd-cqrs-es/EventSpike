using System;

namespace NEventStore.Spike.ApprovalProcessorService.Automatonymous
{
    public interface IApprovalProcessorRepository
    {
        ApprovalProcessorInstance GetProcessorById(Guid id);
    }
}