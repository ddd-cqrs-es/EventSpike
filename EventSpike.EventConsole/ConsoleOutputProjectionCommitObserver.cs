using System;
using EventSpike.Common.EventSubscription;
using NEventStore;
using Newtonsoft.Json;

namespace EventSpike.EventConsole
{
    internal class ConsoleOutputProjectionCommitObserver : IObserver<object>
    {
        private readonly ITrackStoreCheckpoints _storeTrackerProvider;

        public ConsoleOutputProjectionCommitObserver(ITrackStoreCheckpoints storeTrackerProvider)
        {
            _storeTrackerProvider = storeTrackerProvider;
        }

        public void OnNext(object message)
        {
            var commit = message as ICommit;
            if (commit == null) return;

            Console.WriteLine(JsonConvert.SerializeObject(commit));

            _storeTrackerProvider
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