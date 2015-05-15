using System;
using System.Collections.Generic;
using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class CommonDomainApprovalProcessEventHandler :
        IHandler
    {
        private readonly ISagaRepository _repository;
        
        public static readonly string UserId = string.Format("#{0}#", typeof(ApprovalProcessor).Name);

        public CommonDomainApprovalProcessEventHandler(ISagaRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Envelope<ApprovalInitiated> message)
        {
            var saga = _repository.GetById<ApprovalProcessor>(message.Message.Id);

            saga.Transition(message.Message);

            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(message.Headers[Constants.CausationIdKey].ToGuid());
            
            _repository.Save(saga, commitId, headers => SetHeaders(headers, message.Headers, commitId));
        }

        public void Handle(Envelope<ApprovalAccepted> message)
        {
            var saga = _repository.GetById<ApprovalProcessor>(message.Message.Id);

            saga.Transition(message.Message);

            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(message.Headers[Constants.CausationIdKey].ToGuid());

            _repository.Save(saga, commitId, headers => SetHeaders(headers, message.Headers, commitId));
        }

        private static void SetHeaders(IDictionary<string, object> target, MessageHeaders source, Guid commitId)
        {
            target.CopyFrom(source.ToDictionary());
            target[Constants.UserIdKey] = UserId;
            target[Constants.CausationIdKey] = commitId.ToString();
        }
    }
}