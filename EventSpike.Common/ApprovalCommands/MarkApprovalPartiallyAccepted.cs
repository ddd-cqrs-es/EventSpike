using System;

namespace EventSpike.Common.ApprovalCommands
{
    public class MarkApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}