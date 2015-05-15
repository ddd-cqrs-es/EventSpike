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
        private bool _initialized;
        private readonly SqlProjector _projector;

        public ProjacInitializer(ConnectionStringSettings settings)
        {
            _projector = new SqlProjector(ApprovalProcessorProjection.Instance, new TransactionalSqlCommandExecutor(settings, IsolationLevel.ReadCommitted));
        }

        public void Initialize()
        {
            if (_initialized) return;

            _projector.Project(new object[] { new DropSchema(), new CreateSchema() });
            _initialized = true;
        }
    }
}