using System;
using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class CommonDomainApprovalProcessEventHandler :
        IEventHandler
    {
        private readonly ISagaRepository _repository;
        
        public static readonly string UserId = string.Format("#{0}#", typeof(ApprovalProcessor).Name);

        public CommonDomainApprovalProcessEventHandler(ISagaRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Envelope<ApprovalInitiated> message)
        {
            var saga = _repository.GetById<ApprovalProcessor>(message.Body.Id);

            saga.Transition(message.Body);

            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(((Guid)message.Headers[Constants.CausationIdKey]).ToByteArray());

            _repository.Save(saga, commitId, headers => headers.CopyFrom(message.Headers));
        }

        public void Handle(Envelope<ApprovalAccepted> message)
        {
            var saga = _repository.GetById<ApprovalProcessor>(message.Body.Id);

            saga.Transition(message.Body);

            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(((Guid)message.Headers[Constants.CausationIdKey]).ToByteArray());

            _repository.Save(saga, commitId, headers => headers.CopyFrom(message.Headers));
        }
    }
}