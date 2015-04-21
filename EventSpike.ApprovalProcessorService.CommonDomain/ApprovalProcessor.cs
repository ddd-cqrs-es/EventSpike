using System;
using CommonDomain.Core;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessorService.CommonDomain
{
    // This could also make use of Automatonymous or Stateless
    public class ApprovalProcessor :
        SagaBase<object>
    {
        private ApprovalProcessor()
        {
            Register<ApprovalInitiated>(Apply);
            Register<ApprovalAccepted>(Apply);
        }

        public ApprovalProcessor(string id) :
            this()
        {
            Id = id;
        }

        public void Apply(ApprovalInitiated @event)
        {
            Id = ApprovalProcessorConstants.DeterministicGuid.Create(@event.Id.ToByteArray()).ToString();

            Dispatch(new MarkApprovalAccepted {Id = @event.Id, ReferenceNumber = Guid.NewGuid().ToString()});
        }

        public void Apply(ApprovalAccepted @event)
        {
            // Do nothing
        }
    }
}