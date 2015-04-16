using System;

namespace NEventStore.Spike.Common.ApprovalCommands
{
    public class MarkApprovalDenied
    {
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public Guid Id { get; set; }
        public Guid CausationId { get; set; }
        public string ReferenceNumber { get; set; }
        public string DenialReason { get; set; }
    }
}