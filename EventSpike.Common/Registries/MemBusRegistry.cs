using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class MemBusRegistry
        : Registry
    {
        public MemBusRegistry()
        {
            For<IBus>()
                .Use(context => BusSetup.StartWith<Conservative>()
                    .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                    .Construct())
                .OnCreation((context, bus) => WireUpMemBus(context, bus));
        }

        private static void WireUpMemBus(IContext context, ISubscriber bus)
        {
            var handlers = context.GetAllInstances<IHandleEvents>();

            foreach (var handler in handlers)
            {
                bus.Subscribe(handler);
            }
        }
    }
}