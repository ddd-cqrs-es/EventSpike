using System;
using System.Collections.Generic;
using System.Linq;
using AggregateSource;
using NEventStore;
using NEventStore.Persistence;

namespace EventSpike.Approval.AggregateSource.Persistence
{
    public class NEventStoreUnitOfWorkCommitter
    {
        private const string AggregateTypeHeader = "AggregateType";

        private readonly IStoreEvents _eventStore;
        private readonly ConflictsWithDelegate _conflictDetector;
        private readonly DetermisticGuidDelegate _determisticGuid;
        
        public NEventStoreUnitOfWorkCommitter(IStoreEvents eventStore, ConflictsWithDelegate conflictDetector, DetermisticGuidDelegate determisticGuid)
        {
            _eventStore = eventStore;
            _conflictDetector = conflictDetector;
            _determisticGuid = determisticGuid;
        }

        public void Commit(UnitOfWork unitOfWork, Guid commitSetId, Action<IDictionary<string, object>> updateHeaders)
        {
            foreach (var aggregate in unitOfWork.GetChanges())
            {
                var commitId = _determisticGuid(commitSetId, aggregate.Identifier);

                Save(aggregate, commitId, updateHeaders);
            }
        }

        private void Save(Aggregate aggregate, Guid commitId, Action<IDictionary<string, object>> updateHeaders)
        {
            var headers = PrepareHeaders(aggregate.Root, updateHeaders);

            while (true)
            {
                using (var stream = PrepareStream(aggregate, headers))
                {
                    int commitEventCount = stream.CommittedEvents.Count;

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
                        var conflict = ThrowOnConflict(stream, commitEventCount);
                        stream.ClearChanges();

                        if (conflict)
                        {
                            throw new ConflictingCommandException(e.Message, e);
                        }
                    }
                    catch (StorageException e)
                    {
                        throw new PersistenceException(e.Message, e);
                    }
                }
            }
        }

        private IEventStream PrepareStream(Aggregate aggregate, Dictionary<string, object> headers)
        {
            var stream = _eventStore.OpenStream(aggregate.Identifier, minRevision: 0);

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