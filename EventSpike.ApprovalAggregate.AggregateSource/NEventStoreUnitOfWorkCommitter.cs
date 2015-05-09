using System;
using System.Collections.Generic;
using AggregateSource;
using NEventStore;

namespace EventSpike.ApprovalAggregate.AggregateSource
{
    public class NEventStoreUnitOfWorkCommitter
    {
        private readonly IStoreEvents _eventStore;

        public NEventStoreUnitOfWorkCommitter(IStoreEvents eventStore)
        {
            _eventStore = eventStore;
        }

        public void Commit(UnitOfWork unitOfWork, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            foreach (var aggregate in unitOfWork.GetChanges())
            {
            }
        }

        private void Save(Aggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            Save(Bucket.Default, aggregate, commitId, updateHeaders);
        }

        private void Save(string bucketId, Aggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
        }
    }
}