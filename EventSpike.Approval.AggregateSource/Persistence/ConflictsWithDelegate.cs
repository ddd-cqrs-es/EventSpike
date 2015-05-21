using System.Collections.Generic;

namespace EventSpike.Approval.AggregateSource.Persistence
{
    public delegate bool ConflictsWithDelegate(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
}