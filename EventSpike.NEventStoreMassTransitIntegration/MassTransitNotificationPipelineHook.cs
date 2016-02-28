using System.Linq;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using MassTransit;
using NEventStore;

namespace EventSpike.NEventStoreMassTransitIntegration
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
            _bus.Publish(new EventStreamUpdated
            {
                StreamId = committed.StreamId,
                CausationId = committed.Headers[Constants.CausationIdKey].ToString().ToGuid(),
                CheckpointToken = committed.CheckpointToken
            }, context =>
            {
                var filteredHeaders = committed.Headers.Where(tuple => tuple.Key != "AggregateType");

                foreach (var header in filteredHeaders)
                {
                    context.SetHeader(header.Key, header.Value.ToString());
                }
            });
        }
    }
}