using NEventStore;

namespace EventSpike.Common.EventSubscription
{
    public class MassTransitNotificationPipelineHook :
        PipelineHookBase
    {
        private readonly IPublisher _publisher;

        public MassTransitNotificationPipelineHook(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public override void PostCommit(ICommit committed)
        {
            _publisher.Publish(new EventStreamUpdated
            {
                StreamId = committed.StreamId,
                CausationId = committed.CommitId,
                CheckpointToken = committed.CheckpointToken
            });
        }
    }
}