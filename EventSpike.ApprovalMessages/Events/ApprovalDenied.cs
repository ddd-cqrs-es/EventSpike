using System;

namespace EventSpike.ApprovalMessages.Events
{
    public class ApprovalDenied
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DenialReason { get; set; }
    }
}