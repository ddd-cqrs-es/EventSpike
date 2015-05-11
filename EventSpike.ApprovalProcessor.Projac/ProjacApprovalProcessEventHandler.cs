using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.ApprovalEvents;
using EventSpike.Common.EventSubscription;
using Paramol.Executors;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    public class ProjacApprovalProcessorEventHandler :
        IHandler
    {
        private readonly ConnectionStringSettings _settings;
        private readonly IPublisher _publisher;
        private readonly SqlCommandExecutor _queryExecutor;
        private readonly SqlProjection _projection;

        private readonly DisposeCallback _catchupDisposeCallback;
        private AsyncSqlProjector _projector;
        private int _isDispatching;
        private bool _isLive;

        public ProjacApprovalProcessorEventHandler(ConnectionStringSettings settings, IPublisher publisher)
        {
            _settings = settings;
            _publisher = publisher;

            _queryExecutor = new SqlCommandExecutor(settings);

            _projection = ApprovalProcessorProjection.Instance.Concat(StoreCheckpointProjection.Instance);

            var connection = new SqlConnection(settings.ConnectionString);
            connection.Open();

            var projectionExecutor = new ConnectedSqlCommandExecutor(connection);

            _catchupDisposeCallback = new DisposeCallback(() =>
            {
                connection.Close();
                connection.Dispose();
            });

            _projector = new AsyncSqlProjector(_projection, projectionExecutor);
        }

        public void Handle(SubscriptionIsLive @event)
        {
            _catchupDisposeCallback.Dispose();
            _isLive = true;

            _projector = new AsyncSqlProjector(_projection, new TransactionalSqlCommandExecutor(_settings, IsolationLevel.ReadCommitted));

            DispatchCommands();
        }

        public void Handle(Envelope<ApprovalInitiated> @event)
        {
            _projector.ProjectAsync(@event);

            DispatchCommands();

            _projector.ProjectAsync(new SetCheckpoint(@event.Headers[Constants.TenantIdKey], @event.Headers[Constants.StreamCheckpointTokenKey]));
        }

        public void Handle(Envelope<ApprovalAccepted> @event)
        {
            _projector.ProjectAsync(@event);

            _projector.ProjectAsync(new SetCheckpoint(@event.Headers[Constants.TenantIdKey], @event.Headers[Constants.StreamCheckpointTokenKey]));
        }

        private void DispatchCommands()
        {
            if (!_isLive || Interlocked.CompareExchange(ref _isDispatching, 1, 0) != 0) return;

            var candidates = Enumerable.Repeat(new {Id = default(Guid), CausationId = default(Guid)}, 0).ToList();

            using (var reader = _queryExecutor.ExecuteReader(TSql.QueryStatement(@"SELECT [Id], [CausationId] FROM [ApprovalProcess] WHERE [DispatchAcknowledged] = 0 AND ([Dispatched] IS NULL OR [Dispatched] < DATEADD(MINUTE, -5, GETDATE()))")))
            {
                candidates = reader.Cast<IDataRecord>()
                    .Select(record => new { Id = record.GetGuid(0), CausationId = record.GetGuid(1) })
                    .ToList();
            }

            foreach (var candidate in candidates) {
                var newCausationId = ApprovalProcessorConstants.DeterministicGuid.Create(candidate.CausationId);

                _publisher.Publish(new MarkApprovalAccepted
                {
                    Id = candidate.Id,
                    ReferenceNumber = GuidEncoder.Encode(candidate.CausationId)
                }, context => context.Add(Constants.CausationIdKey, newCausationId.ToString()));

                _queryExecutor.ExecuteNonQuery(TSql.NonQueryStatement(@"UPDATE [ApprovalProcess] SET [Dispatched] = GETDATE() WHERE [Id] = @P1", new { P1 = TSql.UniqueIdentifier(candidate.Id) }));
            }

            Interlocked.Exchange(ref _isDispatching, 0);
        }
    }
}