using System;

namespace NEventStore.Spike.ApprovalService
{
    public class ApprovalModifiedWhenLocked : Exception
    {
        public ApprovalModifiedWhenLocked(string message) : base(message)
        {
        }
    }
}