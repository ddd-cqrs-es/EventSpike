using System.Collections.Generic;

namespace EventSpike.Approval.AggregateSourceIntegration.Persistence
{
    public delegate bool ConflictsWithDelegate(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
}