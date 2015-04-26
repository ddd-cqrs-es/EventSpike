using System;
using Automatonymous;

namespace EventSpike.ApprovalProcessor.Automatonymous
{
    public class ApprovalProcessorInstance
    {
        public Guid ApprovalId { get; set; }
        public Guid CausationId { get; set; }
        public State CurrentState { get; set; }
    }
}