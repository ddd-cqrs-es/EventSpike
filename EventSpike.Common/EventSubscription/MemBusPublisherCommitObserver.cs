using System;
using Magnum.Reflection;
using MemBus;
using NEventStore;

namespace EventSpike.Common.EventSubscription
{
    public class MemBusPublisherCommitObserver : IObserver<object>
    {
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
            var envelope = Envelope<TEvent>
                .Create(commit.Headers, @event);

            envelope.Headers[Constants.CausationIdKey] = commit.CommitId.ToString();
            envelope.Headers[Constants.StreamCheckpointTokenKey] = commit.CheckpointToken;

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