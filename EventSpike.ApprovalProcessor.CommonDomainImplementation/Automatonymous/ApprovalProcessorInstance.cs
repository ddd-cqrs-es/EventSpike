using System;
using Automatonymous;

namespace EventSpike.ApprovalProcessor.CommonDomainImplementation.Automatonymous
{
    public class ApprovalProcessorInstance
    {
        public Guid ApprovalId { get; set; }
        public State CurrentState { get; set; }
    }
}