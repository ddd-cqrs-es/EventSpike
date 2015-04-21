using System;

namespace EventSpike.Common.ApprovalCommands
{
    public class MarkApprovalDenied
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DenialReason { get; set; }
    }
}