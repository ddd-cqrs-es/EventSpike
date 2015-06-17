using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EventSpike.Common
{
    /// <summary>
    /// Helper class redirects events to private Apply(event) method
    /// </summary>
    public class ConventionDispatcher
    {
        // Adapted from https://gist.github.com/danbarua/8a9387957f8a0d884f41
        private readonly string _methodName;

        private static readonly IDictionary<Type, IDictionary<Type, MethodInfo>> Cache = new ConcurrentDictionary<Type, IDictionary<Type, MethodInfo>>();

        private static readonly MethodInfo InternalPreserveStackTraceMethod = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

        public ConventionDispatcher(string methodName)
        {
            _methodName = methodName;
        }

        [DebuggerNonUserCode]
        public void Dispatch(object instance, object @event)
        {
            MethodInfo info;
            var instanceType = instance.GetType();
            IDictionary<Type, MethodInfo> lookup;
            if (!Cache.TryGetValue(instanceType, out lookup))
            {
                lookup = instanceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(m => m.Name == _methodName)
                    .Where(m => m.GetParameters().Length == 1)
                    .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);

                Cache[instanceType] = lookup;
            }

            var eventType = @event.GetType();
            if (!lookup.TryGetValue(eventType, out info))
            {
                var s = string.Format("Failed to locate {0}.{1}({2})", instanceType.Name, _methodName, eventType.Name);
                throw new InvalidOperationException(s);
            }

            try
            {
                info.Invoke(instance, new[] { @event });
            }
            catch (TargetInvocationException ex)
            {
                if (null != InternalPreserveStackTraceMethod)
                {
                    InternalPreserveStackTraceMethod.Invoke(ex.InnerException, new object[0]);
                }

                throw ex.InnerException;
            }
        }

        public void DispatchOptional(object instance, object @event)
        {
            MethodInfo info;
            var instanceType = instance.GetType();
            IDictionary<Type, MethodInfo> lookup;
            if (!Cache.TryGetValue(instanceType, out lookup))
            {
                lookup = instanceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(m => m.Name == _methodName)
                    .Where(m => m.GetParameters().Length == 1)
                    .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);

                Cache[instanceType] = lookup;
            }

            var eventType = @event.GetType();
            if (!lookup.TryGetValue(eventType, out info))
            {
                return;
            }

            try
            {
                info.Invoke(instance, new[] { @event });
            }
            catch (TargetInvocationException ex)
            {
                if (null != InternalPreserveStackTraceMethod)
                {
                    InternalPreserveStackTraceMethod.Invoke(ex.InnerException, new object[0]);
                }

                throw ex.InnerException;
            }
        }
    }
}
