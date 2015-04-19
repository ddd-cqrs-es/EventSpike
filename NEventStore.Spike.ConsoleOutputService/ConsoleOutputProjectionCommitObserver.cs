using System;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.CommonDomain;
using NEventStore.Spike.Common.EventSubscription;
using Newtonsoft.Json;

namespace NEventStore.Spike.ConsoleOutputService
{
    internal class ConsoleOutputProjectionCommitObserver : IObserver<ICommit>
    {
        private readonly ITenantProvider<IStreamCheckpointTracker> _streamTrackerProvider;

        public ConsoleOutputProjectionCommitObserver(ITenantProvider<IStreamCheckpointTracker> streamTrackerProvider)
        {
            _streamTrackerProvider = streamTrackerProvider;
        }

        public void OnNext(ICommit commit)
        {
            Console.WriteLine(JsonConvert.SerializeObject(commit));

            _streamTrackerProvider
                .Get(commit.Headers.Retrieve<SystemHeaders>().TenantId)
                .UpdateCheckpoint(commit.CheckpointToken);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(JsonConvert.SerializeObject(error));
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }
    }
}