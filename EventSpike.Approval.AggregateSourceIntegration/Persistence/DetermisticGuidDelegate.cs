using System;

namespace EventSpike.Approval.AggregateSourceIntegration.Persistence
{
    public delegate Guid DetermisticGuidDelegate(Guid commitSetId, string aggregateId);
}