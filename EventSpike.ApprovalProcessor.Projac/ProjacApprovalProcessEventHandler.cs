using System;
using System.Configuration;
using System.Data;
using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;
using Paramol.Executors;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    public class ProjacApprovalProcessorEventHandler :
        IEventHandler
    {
        private readonly IPublisher _publisher;
        private readonly SqlProjector _projector;
        private readonly SqlCommandExecutor _queryExecutor;

        public ProjacApprovalProcessorEventHandler(ConnectionStringSettings settings, IPublisher publisher)
        {
            _publisher = publisher;

            _queryExecutor = new SqlCommandExecutor(settings);

            var projectionExecutor = new TransactionalSqlCommandExecutor(settings, IsolationLevel.ReadCommitted);

            _projector = new SqlProjector(ApprovalProcessorProjection.Instance.Concat(StoreCheckpointProjection.Instance), projectionExecutor);
        }

        public void Handle(Envelope<ApprovalInitiated> @event)
        {
            _projector.Project(@event);

            DispatchCommands();

            _projector.Project(new SetCheckpoint((string)@event.Headers[Constants.TenantIdKey], (string)@event.Headers[Constants.StreamCheckpointTokenKey]));
        }

        public void Handle(Envelope<ApprovalAccepted> @event)
        {
            _projector.Project(@event);

            DispatchCommands();

            _projector.Project(new SetCheckpoint((string)@event.Headers[Constants.TenantIdKey], (string)@event.Headers[Constants.StreamCheckpointTokenKey]));
        }

        private void DispatchCommands()
        {
            using (var reader = _queryExecutor.ExecuteReader(TSql.QueryStatement(@"SELECT [Id], [CausationId] FROM [ApprovalProcess] WHERE [Dispatched] IS NULL OR [Dispatched] < DATEADD(MINUTE, -5, GETDATE())")))
            {
                if (reader.IsClosed) return;
                while (reader.Read())
                {
                    var id = reader.GetGuid(0);
                    var causationId = reader.GetGuid(1);
                    var newCausationId = ApprovalProcessorConstants.DeterministicGuid.Create(causationId.ToByteArray());

                    _publisher.Publish(new MarkApprovalAccepted
                    {
                        Id = id,
                        ReferenceNumber = GuidEncoder.Encode(causationId)
                    }, context => context.Add(Constants.CausationIdKey, newCausationId.ToString()));

                    _queryExecutor.ExecuteNonQueryAsync(TSql.NonQueryStatement(@"UPDATE [ApprovalProcess] SET [Dispatched] = GETDATE() WHERE [Id] = P1", new { P1 = TSql.UniqueIdentifier(id) }));
                }
            }
        }
    }
}