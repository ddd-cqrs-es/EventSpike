using System;
using Automatonymous;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public class ApprovalProcessorInstance
    {
        public Guid ApprovalId { get; set; }
        public Guid CausationId { get; set; }
        public string TenantId { get; set; }
        public State CurrentState { get; set; }
    }
}