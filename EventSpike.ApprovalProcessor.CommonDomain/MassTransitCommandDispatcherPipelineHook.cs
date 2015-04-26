using System.Linq;
using EventSpike.Common;
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
                _publisher.Publish(command);
            }
        }
    }
}