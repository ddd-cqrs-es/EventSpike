using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.Pipeline;
using MassTransit.Saga;
using StructureMap;

namespace EventSpike.Common.MassTransit
{
    public class StructureMapSagaRepository<T> :
        ISagaRepository<T>
        where T : class, ISaga
    {
        readonly IContainer _container;
        readonly ISagaRepository<T> _repository;

        public StructureMapSagaRepository(ISagaRepository<T> repository, IContainer container)
        {
            _repository = repository;
            _container = container;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context,
            Guid sagaId, InstanceHandlerSelector<T, TMessage> selector, ISagaPolicy<T, TMessage> policy)
            where TMessage : class
        {
            var tenantId = context.Headers[Constants.TenantIdKey];

            return _repository
                .GetSaga(context, sagaId, selector, policy)
                .Select(consumer => (Action<IConsumeContext<TMessage>>) (x =>
                {
                    using (var nestedContainer = _container.GetNestedContainer())
                    {
                        nestedContainer
                            .Configure(configure => configure.For<TenantIdProvider>()
                                .Use(new TenantIdProvider(() => tenantId)));

                        consumer(x);
                    }
                }));
        }

        public IEnumerable<Guid> Find(ISagaFilter<T> filter)
        {
            return _repository.Find(filter);
        }

        public IEnumerable<T> Where(ISagaFilter<T> filter)
        {
            return _repository.Where(filter);
        }

        public IEnumerable<TResult> Where<TResult>(ISagaFilter<T> filter, Func<T, TResult> transformer)
        {
            return _repository.Where(filter, transformer);
        }

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> transformer)
        {
            return _repository.Select(transformer);
        }
    }
}