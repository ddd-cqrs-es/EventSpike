using System;

namespace EventSpike.Common.ApprovalCommands
{
    public class MarkApprovalPartiallyAccepted
    {
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public Guid Id { get; set; }
        public Guid CausationId { get; set; }
        public string ReferenceNumber { get; set; }
    }
}