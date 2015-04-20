using System;

namespace EventSpike.Common.ApprovalEvents
{
    public class ApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}