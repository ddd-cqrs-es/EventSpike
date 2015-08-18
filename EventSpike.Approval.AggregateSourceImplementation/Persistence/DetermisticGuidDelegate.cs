using System;

namespace EventSpike.Approval.AggregateSourceImplementation.Persistence
{
    public delegate Guid DetermisticGuidDelegate(Guid commitSetId, string aggregateId);
}