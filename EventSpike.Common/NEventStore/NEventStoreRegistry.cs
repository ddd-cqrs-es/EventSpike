using NEventStore;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.NEventStore
{
    public class NEventStoreRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "EventSpike-{0}",
            SingleTenantConnectionString = "Database=EventSpike;Server=(local);Integrated Security=SSPI;";

        public NEventStoreRegistry()
        {
            For<ConnectionStringEnvelope>()
                .MissingNamedInstanceIs
                .ConstructedBy(context => new ConnectionStringEnvelope(string.Format(TenantConnectionName, context.RequestedName), SingleTenantConnectionString));

            For<IStoreEvents>()
                .Singleton()
                .MissingNamedInstanceIs
                .ConstructedBy(context => context.GetInstance<TenantEventStoreFactory>().Construct(context.RequestedName));

            ForConcreteType<TenantEventStoreFactory>()
                .Configure
                .Singleton();
        }
    }
}