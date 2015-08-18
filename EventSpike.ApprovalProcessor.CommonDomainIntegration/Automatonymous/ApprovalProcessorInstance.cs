using System;
using Automatonymous;

namespace EventSpike.ApprovalProcessor.CommonDomainIntegration.Automatonymous
{
    public class ApprovalProcessorInstance
    {
        public Guid ApprovalId { get; set; }
        public State CurrentState { get; set; }
    }
}