using System;
using System.Reflection;
using CommonDomain;
using CommonDomain.Persistence;

namespace EventSpike.Common.CommonDomain
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var typeParam = snapshot != null ? snapshot.GetType() : typeof (Guid);

            var paramArray = snapshot != null
                ? new object[] {snapshot}
                : new object[] {id};

            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeParam}, null);

            if (constructor == null)
            {
                throw new InvalidOperationException(string.Format("Aggregate {0} cannot be created: constructor with proper parameter not provided", type.Name));
            }

            return constructor.Invoke(paramArray) as IAggregate;
        }
    }
}