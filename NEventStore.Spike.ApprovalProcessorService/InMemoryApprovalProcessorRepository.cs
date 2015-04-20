using System;
using System.Collections.Concurrent;
using Automatonymous;
using MassTransit;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class InMemoryApprovalProcessorRepository :
        IApprovalProcessorRepository
    {
        private readonly IServiceBus _bus;
        private readonly ConcurrentDictionary<Guid, InstanceLift<ApprovalProcessor>> _processorStates = new ConcurrentDictionary<Guid, InstanceLift<ApprovalProcessor>>();

        public InMemoryApprovalProcessorRepository(IServiceBus bus)
        {
            _bus = bus;
        }

        public InstanceLift<ApprovalProcessor> GetProcessorById(Guid id)
        {
            return _processorStates.GetOrAdd(id, newId => new ApprovalProcessor {Bus = _bus}.CreateInstanceLift(new ApprovalProcessorInstance()));
        }
    }
}