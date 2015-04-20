using System;

namespace EventSpike.ApprovalProcessorService.Automatonymous
{
    public interface IApprovalProcessorRepository
    {
        ApprovalProcessorInstance GetProcessorById(Guid id);
    }
}