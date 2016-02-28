using EventSpike.ApprovalMessages.Commands;
using EventSpike.Common;
using MassTransit;

namespace EventSpike.Approval.MassTransitIntegration
{
    public class MassTransitConsumerAdapter :
        Consumes<InitiateApproval>.Context,
        Consumes<MarkApprovalAccepted>.Context,
        Consumes<MarkApprovalCancelled>.Context,
        Consumes<MarkApprovalDenied>.Context,
        Consumes<MarkApprovalPartiallyAccepted>.Context
    {
        private readonly ConventionDispatcher _dispatcher;
        private readonly IHandler _handler;

        public MassTransitConsumerAdapter(IHandler handler)
        {
            _dispatcher = new ConventionDispatcher("Handle");
            _handler = handler;
        }

        public void Consume(IConsumeContext<InitiateApproval> context)
        {
            _dispatcher.Dispatch(_handler, context.ToEnvelope());
        }

        public void Consume(IConsumeContext<MarkApprovalAccepted> context)
        {
            _dispatcher.Dispatch(_handler, context.ToEnvelope());
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            _dispatcher.Dispatch(_handler, context.ToEnvelope());
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            _dispatcher.Dispatch(_handler, context.ToEnvelope());
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            _dispatcher.Dispatch(_handler, context.ToEnvelope());
        }
    }
}
