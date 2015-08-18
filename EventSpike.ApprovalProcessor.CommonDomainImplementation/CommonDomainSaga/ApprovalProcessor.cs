using System;
using CommonDomain.Core;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.CommonDomainImplementation.CommonDomainSaga
{
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

        private void Apply(ApprovalInitiated @event)
        {
            Id = ApprovalProcessorConstants.DeterministicGuid.Create(@event.Id).ToString();

            Dispatch(new MarkApprovalAccepted {Id = @event.Id, ReferenceNumber = GuidEncoder.Encode(Guid.NewGuid())});
        }

        private void Apply(ApprovalAccepted @event)
        {
            // Do nothing
        }
    }
}