using System;
using System.Linq;
using Logary;
using Magnum.Reflection;
using MemBus;
using NEventStore;

namespace EventSpike.Common.EventSubscription
{
    public class MemBusPublisherCommitObserver : IObserver<object>
    {
        private static readonly Logger Logger = Logging.GetCurrentLogger();
        private readonly IBus _bus;

        public MemBusPublisherCommitObserver(IBus bus)
        {
            _bus = bus;
        }

        public void OnNext(object message)
        {
            if (!(message is ICommit))
            {
                _bus.Publish(message);

                return;
            }

            var commit = message as ICommit;

            if (!commit.Headers.ContainsKey("SagaType"))
            {
                DispatchEvents(commit);
            }
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
            var headers = commit.Headers.ToMessageHeaders()
                .Concat(new[]
                {
                    new MessageHeader(Constants.CausationIdKey, commit.Headers[Constants.CausationIdKey].ToString()),
                    new MessageHeader(Constants.StreamCheckpointTokenKey, commit.CheckpointToken)
                }).ToArray();

            var envelope = new Envelope<TEvent>(@event, headers);

            _bus.Publish(envelope);
        }

        public void OnError(Exception error)
        {
            Logger.ErrorException(error.Message, error);
        }

        public void OnCompleted()
        {
            Logger.Warn("Complete");
        }
    }
}