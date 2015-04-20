using System;
using Automatonymous;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public interface IApprovalProcessorRepository
    {
        InstanceLift<ApprovalProcessor> GetProcessorById(Guid id);
    }
}