using System;

namespace EventSpike.ApprovalMessages.Commands
{
    public class MarkApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}