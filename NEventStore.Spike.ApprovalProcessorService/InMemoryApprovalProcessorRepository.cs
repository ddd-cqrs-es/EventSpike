using System;
using System.Collections.Concurrent;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class InMemoryApprovalProcessorRepository :
        IApprovalProcessorRepository
    {
        private readonly ConcurrentDictionary<Guid, ApprovalProcessorInstance> _processorStates = new ConcurrentDictionary<Guid, ApprovalProcessorInstance>();
        
        public ApprovalProcessorInstance GetProcessorById(Guid id)
        {
            return _processorStates.GetOrAdd(id, newId => new ApprovalProcessorInstance());
        }
    }
}