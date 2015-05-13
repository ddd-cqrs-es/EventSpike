using Automatonymous;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.Automatonymous
{
    internal class ApprovalProcessor :
        AutomatonymousStateMachine<ApprovalProcessorInstance>
    {
        private static readonly string UserId = string.Format("#{0}#", typeof (ApprovalProcessor).Name);

        public IPublishMessages Publisher { private get; set; }

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
                        state.ApprovalId = @event.Message.Id;
                        state.CausationId = @event.Headers[Constants.CausationIdKey].ToGuid();
                    })
                    .TransitionTo(WaitingForApproval));

            During(WaitingForApproval,
                When(WaitingForApproval.Enter)
                    .Then(state => Publisher.Publish(new MarkApprovalAccepted
                    {
                        Id = state.ApprovalId,
                        ReferenceNumber = GuidEncoder.Encode(state.CausationId)
                    }, headers =>
                    {
                        headers.Add(Constants.CausationIdKey, ApprovalProcessorConstants.DeterministicGuid.Create(state.CausationId).ToString());
                        headers.Add(Constants.UserIdKey, UserId);
                    })),
                When(Accepted)
                    .Finalize());
        }

        public Event<Envelope<ApprovalInitiated>> Initiated { get; set; }

        public Event<Envelope<ApprovalAccepted>> Accepted { get; set; }

        public State WaitingForApproval { get; set; }

        public State Completed { get; set; }
    }
}