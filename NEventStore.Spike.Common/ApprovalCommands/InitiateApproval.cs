using System;

namespace NEventStore.Spike.Common.ApprovalCommands
{
    public class InitiateApproval
    {
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public Guid Id { get; set; }
        public Guid CausationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
