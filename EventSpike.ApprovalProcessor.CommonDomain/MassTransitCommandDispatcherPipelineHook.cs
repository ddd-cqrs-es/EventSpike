using System.Linq;
using EventSpike.Common;
using Magnum.Reflection;
using NEventStore;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class CommandPublisherPipelineHook :
        PipelineHookBase
    {
        private readonly IPublisher _publisher;
        // This could also be dispatched via a PollingConsumer

        public CommandPublisherPipelineHook(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public override void PostCommit(ICommit committed)
        {
            if (!committed.Headers.ContainsKey("SagaType"))
                return;

            var commands = committed.Headers
                .Where(header => header.Key.StartsWith("UndispatchedMessage."))
                .Select(envelope => envelope.Value).ToList();

            foreach (var command in commands)
            {
                this.FastInvoke(new[] { command.GetType() }, x => x.Publish(default(ICommit), default(object)), committed, command);
            }
        }

        private void Publish<TCommand>(ICommit commit, TCommand command)
        {

            _publisher.Publish(command);
        }
    }
}