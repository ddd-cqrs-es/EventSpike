using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using MassTransit;

namespace EventSpike.ApprovalAggregate.CommonDomain
{
    public class MassTransitApprovalCommandConsumer :
        Consumes<InitiateApproval>.Context,
        Consumes<MarkApprovalAccepted>.Context,
        Consumes<MarkApprovalPartiallyAccepted>.Context,
        Consumes<MarkApprovalDenied>.Context,
        Consumes<MarkApprovalCancelled>.Context
    {
        private readonly ITenantProvider<IRepository> _repositoryProvider;

        public MassTransitApprovalCommandConsumer(ITenantProvider<IRepository> repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
        }

        public void Consume(IConsumeContext<InitiateApproval> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = new ApprovalAggregate(context.Message.Id, context.Message.Title, context.Message.Description);

            repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalAccepted> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkAccepted(context.Message.ReferenceNumber);

            repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.Cancel(context.Message.CancellationReason);

            repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkDenied(context.Message.ReferenceNumber, context.Message.DenialReason);

            repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.GetById<ApprovalAggregate>(context.Message.Id);

            approval.MarkPartiallyAccepted(context.Message.ReferenceNumber);

            repository.Save(approval, causationId, headers => headers.CopyFrom(context.Headers));
        }
    }
}