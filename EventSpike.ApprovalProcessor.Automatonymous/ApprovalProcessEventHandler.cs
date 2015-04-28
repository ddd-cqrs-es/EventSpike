using Automatonymous;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.Automatonymous
{
    public class AutomatonymousApprovalProcessEventHandler :
        IEventHandler
    {
        private readonly IPublisher _publisher;
        private readonly IApprovalProcessorRepository _repository;

        public AutomatonymousApprovalProcessEventHandler(IPublisher publisher, IApprovalProcessorRepository repository)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public void Handle(Envelope<ApprovalInitiated> message)
        {
            var processorInstance = _repository.GetProcessorById(message.Message.Id);

            var processor = new ApprovalProcessor {Publisher = _publisher};

            processor.RaiseEvent(processorInstance, eventIs => eventIs.Initiated, message);
        }

        public void Handle(Envelope<ApprovalAccepted> message)
        {
            var processorInstance = _repository.GetProcessorById(message.Message.Id);

            var processor = new ApprovalProcessor {Publisher = _publisher};

            processor.RaiseEvent(processorInstance, eventIs => eventIs.Accepted, message);
        }
    }
}
