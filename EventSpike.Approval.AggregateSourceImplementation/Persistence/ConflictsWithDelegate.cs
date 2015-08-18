using System.Collections.Generic;

namespace EventSpike.Approval.AggregateSourceImplementation.Persistence
{
    public delegate bool ConflictsWithDelegate(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
}