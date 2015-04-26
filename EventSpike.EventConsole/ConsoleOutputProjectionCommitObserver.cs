using System;
using EventSpike.Common.EventSubscription;
using NEventStore;
using Newtonsoft.Json;

namespace EventSpike.EventConsole
{
    internal class ConsoleOutputProjectionCommitObserver : IObserver<ICommit>
    {
        private readonly IStreamCheckpointTracker _streamTrackerProvider;

        public ConsoleOutputProjectionCommitObserver(IStreamCheckpointTracker streamTrackerProvider)
        {
            _streamTrackerProvider = streamTrackerProvider;
        }

        public void OnNext(ICommit commit)
        {
            Console.WriteLine(JsonConvert.SerializeObject(commit));

            _streamTrackerProvider
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