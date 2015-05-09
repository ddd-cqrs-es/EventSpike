using System;
using System.Collections.Generic;
using System.Linq;
using AggregateSource;
using NEventStore;
using NEventStore.Persistence;

namespace EventSpike.Approval.AggregateSource.Persistence
{
    public delegate bool ConflictsWithDelegate(IEnumerable<object> uncommittedEvents, IEnumerable<object> committedEvents);
    public delegate Guid DetermisticGuidDelegate(Guid commitSetId, string aggregateId);
    
    public class NEventStoreUnitOfWorkCommitter
    {
        private const string AggregateTypeHeader = "AggregateType";

        private readonly IStoreEvents _eventStore;
        private readonly ConflictsWithDelegate _conflictDetector;
        private readonly DetermisticGuidDelegate _determisticGuid;

        private readonly IDictionary<string, IEventStream> _streams = new Dictionary<string, IEventStream>();

        public NEventStoreUnitOfWorkCommitter(IStoreEvents eventStore, ConflictsWithDelegate conflictDetector, DetermisticGuidDelegate determisticGuid)
        {
            _eventStore = eventStore;
            _conflictDetector = conflictDetector;
            _determisticGuid = determisticGuid;
        }

        public void Commit(UnitOfWork unitOfWork, Guid commitSetId, Action<IDictionary<string, object>> updateHeaders)
        {
            Commit(Bucket.Default, unitOfWork, commitSetId, updateHeaders);
        }

        public void Commit(string bucketId, UnitOfWork unitOfWork, Guid commitSetId, Action<IDictionary<string, object>> updateHeaders)
        {
            foreach (var aggregate in unitOfWork.GetChanges())
            {
                var commitId = _determisticGuid(commitSetId, aggregate.Identifier);

                Save(bucketId, aggregate, commitId, updateHeaders);
            }
        }

        private void Save(string bucketId, Aggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var headers = PrepareHeaders(aggregate.Root, updateHeaders);

            while (true)
            {
                var stream = PrepareStream(bucketId, aggregate, headers);

                try
                {
                    stream.CommitChanges(commitId);
                    aggregate.Root.ClearChanges();
                    return;
                }
                catch (DuplicateCommitException)
                {
                    stream.ClearChanges();
                    return;
                }
                catch (ConcurrencyException e)
                {
                    stream.ClearChanges();

                    throw new ConflictingCommandException(e.Message, e);
                }
                catch (StorageException e)
                {
                    throw new PersistenceException(e.Message, e);
                }
            }
        }

        private IEventStream PrepareStream(string bucketId, Aggregate aggregate, Dictionary<string, object> headers)
        {
            IEventStream stream;
            var streamId = bucketId + "+" + aggregate.Identifier;
            if (!_streams.TryGetValue(streamId, out stream))
            {
                _streams[streamId] = stream = _eventStore.CreateStream(bucketId, aggregate.Identifier);
            }

            foreach (var item in headers)
            {
                stream.UncommittedHeaders[item.Key] = item.Value;
            }

            aggregate.Root.GetChanges()
                .Select(x => new EventMessage {Body = x})
                .ToList()
                .ForEach(stream.Add);

            return stream;
        }
        
        private static Dictionary<string, object> PrepareHeaders(IAggregateRootEntity aggregate, Action<IDictionary<string, object>> updateHeaders)
        {
            var headers = new Dictionary<string, object>();

            headers[AggregateTypeHeader] = aggregate.GetType().FullName;
            if (updateHeaders != null)
            {
                updateHeaders(headers);
            }

            return headers;
        }

        private bool ThrowOnConflict(IEventStream stream, int skip)
        {
            var committed = stream.CommittedEvents.Skip(skip).Select(x => x.Body);
            var uncommitted = stream.UncommittedEvents.Select(x => x.Body);
            return _conflictDetector(uncommitted, committed);
        }
    }
}