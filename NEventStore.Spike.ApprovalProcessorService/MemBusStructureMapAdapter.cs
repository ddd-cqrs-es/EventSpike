using System;
using System.Collections.Generic;
using System.Linq;
using MemBus;
using StructureMap;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public class MemBusStructureMapAdapter :
        IocAdapter
    {
        private readonly IContainer _container;

        public MemBusStructureMapAdapter(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            return _container.GetAllInstances(desiredType).Cast<object>();
        }
    }
}