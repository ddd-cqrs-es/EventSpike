using System.Collections.Generic;
using Autofac;
using Autofac.Extras.Multitenant;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;

namespace EventSpike.Common.Autofac
{
    public class MemBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var bus = BusSetup.StartWith<Conservative>()
                    .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                    .Construct();

                var handlers = context.Resolve<IEnumerable<IHandler>>();

                foreach (var handler in handlers)
                {
                    bus.Subscribe(handler);
                }

                return bus;
            }).As<IBus>().InstancePerTenant();
        }
    }
}