using System;
using EventSpike.Common.EventSubscription;
using Logary;
using NEventStore;
using Newtonsoft.Json;

namespace EventSpike.EventConsole
{
    internal class ConsoleOutputProjectionCommitObserver : IObserver<object>
    {
        private static readonly Logger Logger = Logging.GetCurrentLogger();
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

            _storeTrackerProvider.UpdateCheckpoint(commit.CheckpointToken);
        }

        public void OnError(Exception error)
        {
            Logger.ErrorException(error.Message, error);
        }

        public void OnCompleted()
        {
            Logger.Warn("Completed");
        }
    }
}