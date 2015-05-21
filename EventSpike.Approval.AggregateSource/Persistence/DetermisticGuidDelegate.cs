using System;

namespace EventSpike.Approval.AggregateSource.Persistence
{
    public delegate Guid DetermisticGuidDelegate(Guid commitSetId, string aggregateId);
}