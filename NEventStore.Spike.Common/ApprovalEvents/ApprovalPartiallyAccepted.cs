using System;

namespace NEventStore.Spike.Common.ApprovalEvents
{
    public class ApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}