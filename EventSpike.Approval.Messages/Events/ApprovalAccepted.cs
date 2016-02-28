using System;

namespace EventSpike.Approval.Messages.Events
{
    public class ApprovalAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}