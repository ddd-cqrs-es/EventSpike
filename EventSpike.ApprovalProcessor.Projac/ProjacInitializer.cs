using System.Configuration;
using System.Data;
using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using EventSpike.Common;
using Paramol.Executors;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    public class ProjacInitializer :
        INeedInitialization
    {
        private readonly SqlProjector _projector;

        public ProjacInitializer(ConnectionStringSettings settings)
        {
            _projector = new SqlProjector(ApprovalProcessorProjection.Instance.Concat(StoreCheckpointProjection.Instance), new TransactionalSqlCommandExecutor(settings, IsolationLevel.ReadCommitted));
        }

        public void Initialize()
        {
            _projector.Project(new object[] { new DropSchema(), new CreateSchema() });
        }
    }
}