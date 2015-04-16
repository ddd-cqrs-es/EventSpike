using System;

namespace NEventStore.Spike.Common.ApprovalEvents
{
    public class ApprovalDenied
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DenialReason { get; set; }
    }
}