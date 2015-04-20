using System;
using Automatonymous;
using MassTransit;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.ApprovalCommands;
using NEventStore.Spike.Common.ApprovalEvents;
using NEventStore.Spike.Common.CommonDomain;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public class ApprovalProcessor :
        AutomatonymousStateMachine<ApprovalProcessorInstance>
    {
        private static readonly DeterministicGuid DeterministicGuid = new DeterministicGuid(GuidNamespaces.ApprovalProcessor);

        public static readonly string UserId = string.Format("#{0}#", typeof (ApprovalProcessor).Name);

        public IServiceBus Bus { get; set; }

        public ApprovalProcessor()
        {
            InstanceState(state => state.CurrentState);

            Event(() => Initiated);

            State(() => WaitingForApproval);

            Initially(
                When(Initiated)
                    .Then((state, @event) =>
                    {
                        state.ApprovalId = @event.Body.Id;
                        state.CausationId = @event.Headers.Retrieve<ContextHeaders>().CausationId;
                        state.TenantId = @event.Headers.Retrieve<SystemHeaders>().TenantId;
                    })
                    .TransitionTo(WaitingForApproval),
                When(WaitingForApproval.Enter)
                    .Then(state => Bus.Publish(new MarkApprovalAccepted
                    {
                        Id = state.ApprovalId,
                        CausationId = DeterministicGuid.Create(state.CausationId.ToByteArray()),
                        ReferenceNumber = Guid.NewGuid().ToString(),
                        TenantId = state.TenantId,
                        UserId = UserId
                    })));
        }

        public Event<IEnvelope<ApprovalInitiated>> Initiated { get; private set; }

        public State WaitingForApproval { get; private set; }
    }
}