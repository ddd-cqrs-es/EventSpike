using System;

namespace NEventStore.Spike.Common.ApprovalEvents
{
    public class ApprovalAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}