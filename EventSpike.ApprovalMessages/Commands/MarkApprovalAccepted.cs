using System;

namespace EventSpike.ApprovalMessages.Commands
{
    public class MarkApprovalAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}