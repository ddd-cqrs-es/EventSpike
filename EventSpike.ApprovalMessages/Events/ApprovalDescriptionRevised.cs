using System;

namespace EventSpike.ApprovalMessages.Events
{
    public class ApprovalDescriptionRevised
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}