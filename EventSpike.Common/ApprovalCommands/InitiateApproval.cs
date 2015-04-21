using System;

namespace EventSpike.Common.ApprovalCommands
{
    public class InitiateApproval
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}