using System;

namespace EventSpike.Approval.Messages.Events
{
    public class ApprovalInitiated
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}