using System;

namespace EventSpike.ApprovalService
{
    public class ApprovalModifiedWhenLocked : Exception
    {
        public ApprovalModifiedWhenLocked(string message) : base(message)
        {
        }
    }
}