using System.Linq;
using EventSpike.Common;
using EventSpike.Common.CommonDomain;
using Magnum.Reflection;
using MassTransit;
using NEventStore;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class MassTransitCommandDispatcherPipelineHook :
        PipelineHookBase
    {
        // This could also be dispatched via a PollingConsumer
        private readonly IServiceBus _bus;

        public MassTransitCommandDispatcherPipelineHook(IServiceBus bus)
        {
            _bus = bus;
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
            var systemHeaders = commit.Headers.Retrieve<SystemHeaders>();

            var envelope = new CommandEnvelope<TCommand>
            {
                TenantId = systemHeaders.TenantId,
                CausationId = commit.CommitId,
                UserId = systemHeaders.UserId,
                Body = command
            };

            _bus.Publish(envelope);
        }
    }
}