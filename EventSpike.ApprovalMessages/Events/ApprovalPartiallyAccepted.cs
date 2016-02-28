using System;

namespace EventSpike.ApprovalMessages.Events
{
    public class ApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}