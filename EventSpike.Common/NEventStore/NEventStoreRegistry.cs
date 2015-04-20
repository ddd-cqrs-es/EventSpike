using NEventStore;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.NEventStore
{
    public class NEventStoreRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "NEventStoreSpike-{0}",
            SingleTenantConnectionString = "Database=NEventStoreSpike;Server=(local);Integrated Security=SSPI;";

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