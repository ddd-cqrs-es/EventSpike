using System;

namespace EventSpike.Approval.Messages.Events
{
    public class ApprovalDescriptionRevised
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}