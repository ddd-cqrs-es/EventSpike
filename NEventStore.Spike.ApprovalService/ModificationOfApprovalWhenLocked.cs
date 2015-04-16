using System;

namespace NEventStore.Spike.ApprovalService
{
    public class ModificationOfApprovalWhenLocked : Exception
    {
        public ModificationOfApprovalWhenLocked(string message) : base(message)
        {
        }
    }
}