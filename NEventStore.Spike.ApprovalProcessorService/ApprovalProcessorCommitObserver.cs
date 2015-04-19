using System;
using Automatonymous;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.CommonDomain;
using NEventStore.Spike.Common.EventSubscription;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class ApprovalProcessorCommitObserver : IObserver<ICommit>
    {
        private readonly ITenantProvider<IStreamCheckpointTracker> _streamTrackerProvider;
        private readonly ITenantProvider<IApprovalProcessorRepository> _repositoryProvider;

        public ApprovalProcessorCommitObserver(ITenantProvider<IStreamCheckpointTracker> streamTrackerProvider, ITenantProvider<IApprovalProcessorRepository> repositoryProvider)
        {
            _streamTrackerProvider = streamTrackerProvider;
            _repositoryProvider = repositoryProvider;
        }

        public void OnNext(ICommit commit)
        {

        }

        private void Dispatch<TEvent>(ICommit commit, TEvent @event)
        {
            var envelope = Envelope<TEvent>
                .Create(commit.Headers, @event)
                .AddHeader(new ContextHeaders { CausationId = commit.CommitId });

            var processor = _repositoryProvider
                .Get(commit.Headers.Retrieve<SystemHeaders>().TenantId)
                .GetProcessorById(commit.StreamId);

            //processor.RaiseEvent(processor, x => x.Initiated, envelope);

            _streamTrackerProvider
                .Get(commit.Headers.Retrieve<SystemHeaders>().TenantId)
                .UpdateCheckpoint(commit.CheckpointToken);
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