using System.Configuration;
using System.Data;
using EventSpike.ApprovalProcessor.ProjacIntegration.DataDefinition;
using EventSpike.Common;
using Paramol.Executors;
using Projac;

namespace EventSpike.ApprovalProcessor.ProjacIntegration
{
    public class ProjacInitializer :
        INeedInitialization
    {
        private readonly SqlProjector _projector;

        public ProjacInitializer(ConnectionStringSettings settings)
        {
            _projector = new SqlProjector(ApprovalProcessorProjection.Instance, new TransactionalSqlCommandExecutor(settings, IsolationLevel.ReadCommitted));
        }

        public void Initialize()
        {
            _projector.Project(new object[] { new DropSchema(), new CreateSchema() });
        }
    }
}