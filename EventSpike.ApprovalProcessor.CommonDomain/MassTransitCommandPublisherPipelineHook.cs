using System.Linq;
using EventSpike.Common;
using MassTransit;
using NEventStore;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class MassTransitCommandPublisherPipelineHook :
        PipelineHookBase
    {
        private readonly IServiceBus _publisher;
        // This could also be dispatched via a PollingConsumer

        public MassTransitCommandPublisherPipelineHook(IServiceBus publisher)
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
                var filteredHeaders = committed.Headers
                    .Where(tuple => !tuple.Key.StartsWith("UndispatchedMessage."))
                    .Where(tuple => tuple.Key != "AggregateType")
                    .Where(tuple => tuple.Key != "SagaType");

                _publisher.Publish(command, (IPublishContext context) =>
                {
                    foreach (var tuple in filteredHeaders)
                    {
                        context.SetHeader(tuple.Key, tuple.Value.ToString());
                    }
                });
            }
        }
    }
}