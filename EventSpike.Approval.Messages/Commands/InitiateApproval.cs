using System;

namespace EventSpike.Approval.Messages.Commands
{
    public class InitiateApproval
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}