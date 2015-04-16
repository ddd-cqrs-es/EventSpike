using System;

namespace NEventStore.Spike.Common.ApprovalEvents
{
    public class ApprovalInitiated
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
