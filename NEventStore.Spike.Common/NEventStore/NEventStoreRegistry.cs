using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common.NEventStore
{
    public class NEventStoreRegistry :
        Registry
    {
        private const string SingleTenantConnectionString =
            "Application Name=NEventStoreSpike-{0};Database=NEventStoreSpike;Server=(local);Integrated Security=SSPI;";

        public NEventStoreRegistry()
        {
            For<ConnectionStringEnvelope>()
                .MissingNamedInstanceIs
                .ConstructedBy(context => new ConnectionStringEnvelope(string.Format(SingleTenantConnectionString, context.RequestedName)));

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