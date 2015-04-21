using System;
using EventSpike.Common.CommonDomain;
using Magnum.Reflection;
using MemBus;
using NEventStore;

namespace EventSpike.Common.EventSubscription
{
    public class MemBusPublisherCommitObserver : IObserver<ICommit>
    {
        private readonly ITenantProvider<IStreamCheckpointTracker> _streamTrackerProvider;
        private readonly IBus _bus;

        public MemBusPublisherCommitObserver(ITenantProvider<IStreamCheckpointTracker> streamTrackerProvider, IBus bus)
        {
            _streamTrackerProvider = streamTrackerProvider;
            _bus = bus;
        }

        public void OnNext(ICommit commit)
        {
            if (!commit.Headers.ContainsKey("SagaType"))
            {
                DispatchEvents(commit);
            }
            
            var tenantId = commit.Headers.Retrieve<SystemHeaders>().TenantId;
            
            _streamTrackerProvider
                .Get(tenantId)
                .UpdateCheckpoint(commit.CheckpointToken);
        }

        private void DispatchEvents(ICommit commit)
        {
            foreach (var @event in commit.Events)
            {
                this.FastInvoke(new[] { @event.Body.GetType() }, x => x.Publish(default(ICommit), default(object)), commit, @event.Body);
            }
        }

        private void Publish<TEvent>(ICommit commit, TEvent @event)
        {
            var envelope = Envelope<TEvent>
                .Create(commit.Headers, @event)
                .AddHeader(new ContextHeaders {CausationId = commit.CommitId});

            _bus.Publish(envelope);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}