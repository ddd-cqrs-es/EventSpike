using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using MassTransit;

namespace EventSpike.Approval.CommonDomain
{
    public class MassTransitApprovalCommandConsumer :
        Consumes<InitiateApproval>.Context,
        Consumes<MarkApprovalAccepted>.Context,
        Consumes<MarkApprovalPartiallyAccepted>.Context,
        Consumes<MarkApprovalDenied>.Context,
        Consumes<MarkApprovalCancelled>.Context
    {
        private readonly IRepository _repository;

        public MassTransitApprovalCommandConsumer(IRepository repository)
        {
            _repository = repository;
        }

        public void Consume(IConsumeContext<InitiateApproval> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = new ApprovalAggregate(context.Message.Id, context.Message.Title, context.Message.Description);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalAccepted> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkAccepted(context.Message.ReferenceNumber);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.Cancel(context.Message.CancellationReason);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkDenied(context.Message.ReferenceNumber, context.Message.DenialReason);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkPartiallyAccepted(context.Message.ReferenceNumber);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }
    }
}