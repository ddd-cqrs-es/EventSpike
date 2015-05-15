using Autofac;
using EventSpike.ApprovalProcessor.Automatonymous;
using EventSpike.Common;
using EventSpike.Common.Autofac;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class AutomatonymousApprovalProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<BiggyStreamCheckpointModule>();

            builder.RegisterType<AutomatonymousApprovalProcessEventHandler>().As<IHandler>();
        }
    }
}