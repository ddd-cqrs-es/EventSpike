using System;
using EventSpike.Common;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.EventSubscription;
using NEventStore;
using Newtonsoft.Json;

namespace EventSpike.ConsoleOutputService
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