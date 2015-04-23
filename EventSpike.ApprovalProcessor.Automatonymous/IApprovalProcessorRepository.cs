using System;

namespace EventSpike.ApprovalProcessor.Automatonymous
{
    public interface IApprovalProcessorRepository
    {
        ApprovalProcessorInstance GetProcessorById(Guid id);
    }
}