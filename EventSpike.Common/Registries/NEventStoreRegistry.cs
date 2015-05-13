using System.Configuration;
using NEventStore;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace EventSpike.Common.Registries
{
    public class NEventStoreRegistry :
        Registry
    {
        public NEventStoreRegistry()
        {
            For<NEventStoreFactory>()
                .Use<NEventStoreFactory>();

            For<IStoreEvents>()
                .Use(context => context.GetInstance<NEventStoreFactory>().Create())
                .LifecycleIs<ContainerLifecycle>();

            For<NEventStoreFactory>()
                .Add<NEventStoreFactory>()
                .Ctor<ConnectionStringSettings>().Named("Projections")
                .Named("Projections");

            For<IStoreEvents>()
                .Add(context => context.GetInstance<NEventStoreFactory>("Projections").Create())
                .LifecycleIs<ContainerLifecycle>()
                .Named("Projections");
        }
    }
}