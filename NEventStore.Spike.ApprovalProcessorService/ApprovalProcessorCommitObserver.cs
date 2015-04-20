using System;
using Magnum.Reflection;
using MemBus;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.CommonDomain;
using NEventStore.Spike.Common.EventSubscription;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class ApprovalProcessorCommitObserver : IObserver<ICommit>
    {
        private readonly ITenantProvider<IStreamCheckpointTracker> _streamTrackerProvider;
        private readonly IBus _bus;

        public ApprovalProcessorCommitObserver(ITenantProvider<IStreamCheckpointTracker> streamTrackerProvider, IBus bus)
        {
            _streamTrackerProvider = streamTrackerProvider;
            _bus = bus;
        }

        public void OnNext(ICommit commit)
        {
            foreach (var @event in commit.Events)
            {
                this.FastInvoke(new[] {@event.GetType()}, x => x.Publish(default(ICommit), default(object)), commit, @event);
            }
            
            var tenantId = commit.Headers.Retrieve<SystemHeaders>().TenantId;
            
            _streamTrackerProvider
                .Get(tenantId)
                .UpdateCheckpoint(commit.CheckpointToken);
        }

        private void Publish<TEvent>(ICommit commit, TEvent @event)
        {
            var envelope = Envelope<TEvent>
                .Create(commit.Headers, @event)
                .AddHeader(new ContextHeaders { CausationId = commit.CommitId });

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