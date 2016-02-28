using System;

namespace EventSpike.ApprovalMessages.Events
{
    public class ApprovalAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}