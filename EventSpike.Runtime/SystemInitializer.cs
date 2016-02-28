using System.Collections.Generic;

namespace EventSpike.Runtime
{
    public class SystemInitializer : ISystemInitializer
    {
        private readonly IEnumerable<INeedInitialization> _initializers;

        public SystemInitializer(IEnumerable<INeedInitialization> initializers)
        {
            _initializers = initializers;
        }

        public void Initialize()
        {
            foreach (var initializer in _initializers)
            {
                initializer.Initialize();
            }
        }
    }
}