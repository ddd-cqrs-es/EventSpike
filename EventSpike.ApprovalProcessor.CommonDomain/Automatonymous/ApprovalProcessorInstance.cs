using System;
using Automatonymous;

namespace EventSpike.ApprovalProcessor.CommonDomain.Automatonymous
{
    public class ApprovalProcessorInstance
    {
        public Guid ApprovalId { get; set; }
        public State CurrentState { get; set; }
    }
}