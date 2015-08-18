using System;
using Automatonymous;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.CommonDomainImplementation.Automatonymous
{
    public class ApprovalProcessor :
        EventSourcedAutomatonymousStateMachine<ApprovalProcessorInstance>
    {
        public ApprovalProcessor(string id) : this()
        {
            Id = id;
        }

        public ApprovalProcessor()
        {
            InstanceState(state => state.CurrentState);

            Event(() => Initiated);

            Event(() => Accepted);

            State(() => WaitingForApproval);

            Initially(
                When(Initiated)
                    .Then((state, @event) =>
                    {
                        state.ApprovalId = @event.Id;
                    })
                    .TransitionTo(WaitingForApproval));

            During(WaitingForApproval,
                When(WaitingForApproval.Enter)
                    .Then(state => Dispatch(new MarkApprovalAccepted
                    {
                        Id = state.ApprovalId,
                        ReferenceNumber = GuidEncoder.Encode(Guid.NewGuid())
                    })),
                When(Accepted)
                    .Finalize());
        }

        public Event<ApprovalInitiated> Initiated { get; set; }

        public Event<ApprovalAccepted> Accepted { get; set; }

        public State WaitingForApproval { get; set; }

        public State Completed { get; set; }
    }
}