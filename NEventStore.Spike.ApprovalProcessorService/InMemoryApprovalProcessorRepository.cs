using System.Collections.Concurrent;
using MassTransit;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class InMemoryApprovalProcessorRepository :
        IApprovalProcessorRepository
    {
        private readonly IServiceBus _bus;
        private readonly ConcurrentDictionary<string, ApprovalProcessor> _processorStates = new ConcurrentDictionary<string, ApprovalProcessor>();

        public InMemoryApprovalProcessorRepository(IServiceBus bus)
        {
            _bus = bus;
        }

        public ApprovalProcessor GetProcessorById(string id)
        {
            return _processorStates.GetOrAdd(id, newId => new ApprovalProcessor {Bus = _bus});
        }
    }
}