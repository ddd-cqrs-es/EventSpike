using NEventStore;

namespace EventSpike.Common.EventSubscription
{
    public class MassTransitNotificationPipelineHook :
        PipelineHookBase
    {
        private readonly IPublishMessages _publisher;

        public MassTransitNotificationPipelineHook(IPublishMessages publisher)
        {
            _publisher = publisher;
        }

        public override void PostCommit(ICommit committed)
        {
            _publisher.Publish(new EventStreamUpdated
            {
                StreamId = committed.StreamId,
                CausationId = committed.Headers[Constants.CausationIdKey].ToString().ToGuid(),
                CheckpointToken = committed.CheckpointToken
            });
        }
    }
}