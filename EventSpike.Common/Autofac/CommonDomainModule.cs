using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.EventSubscription;
using NEventStore;

namespace EventSpike.Common.Autofac
{
    public class CommonDomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AggregateFactory>().As<IConstructAggregates>();

            builder.RegisterType<SagaFactory>().As<IConstructSagas>();

            builder.RegisterType<MassTransitNotificationPipelineHook>().As<IPipelineHook>();

            builder.RegisterType<EventStoreRepository>().As<IRepository>().InstancePerTenant();

            builder.RegisterType<SagaEventStoreRepository>()
                .WithParameter(ResolvedParameter.ForNamed<IStoreEvents>("Projections"))
                .As<ISagaRepository>();

            builder.RegisterType<ConflictDetector>().As<IDetectConflicts>().OnActivated(@event =>
            {
                var configurations = @event.Context.Resolve<IEnumerable<Action<ConflictDetector>>>();

                foreach (var configure in configurations)
                {
                    configure(@event.Instance);
                }
            });
        }
    }
}