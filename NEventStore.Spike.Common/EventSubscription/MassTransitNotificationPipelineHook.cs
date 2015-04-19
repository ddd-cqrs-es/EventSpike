using MassTransit;
using NEventStore.Spike.Common.CommonDomain;

namespace NEventStore.Spike.Common.EventSubscription
{
    public class MassTransitNotificationPipelineHook :
        PipelineHookBase
    {
        private readonly IServiceBus _bus;

        public MassTransitNotificationPipelineHook(IServiceBus bus)
        {
            _bus = bus;
        }

        public override void PostCommit(ICommit committed)
        {
            var systemHeaders = committed.Headers.Retrieve<SystemHeaders>();

            _bus.Publish(new EventStreamUpdated
            {
                StreamId = committed.StreamId,
                CausationId = committed.CommitId,
                CheckpointToken = committed.CheckpointToken,
                TenantId = systemHeaders.TenantId
            });
        }
    }
}