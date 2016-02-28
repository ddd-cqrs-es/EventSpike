using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace EventSpike
{
    internal class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type eventType) : base(string.Format("No handler found for {0}", eventType.Name))
        {
        }
    }

    internal class Dispatcher<TMessage, TResult>
    {
        private readonly Dictionary<Type, Func<TMessage, TResult>> _dictionary = new Dictionary<Type, Func<TMessage, TResult>>();

        public void Register<T>(Func<T, TResult> handler) where T : TMessage
        {
            _dictionary.Add(typeof(T), message => handler((T)message));
        }

        public TResult Dispatch(TMessage message)
        {
            Func<TMessage, TResult> handler;
            if (!_dictionary.TryGetValue(message.GetType(), out handler))
            {
                throw new HandlerNotFoundException(message.GetType());
            }

            return handler(message);
        }
    }

    internal class Nothing
    {
        private Nothing() { }
        public static readonly Nothing Value = new Nothing();
    }

    //http://devtalk.net/csharp/chained-null-checks-and-the-maybe-monad/
    internal static class ExtensionsForMaybeMonad
    {
        public static bool IsDefault<TInput>(this TInput o)
        {
            return Comparer<TInput>.Default.Compare(o, default(TInput)) == 0;
        }

        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
        {
            return o.IsDefault() ? default(TResult) : evaluator(o);
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            return o.IsDefault() ? failureValue : evaluator(o);
        }

        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
        {
            return o.IsDefault()
                ? default(TInput)
                : evaluator(o) ? o : default(TInput);
        }

        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
        {
            return o.IsDefault()
                ? default(TInput)
                : evaluator(o) ? default(TInput) : o;
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
        {
            if (o.IsDefault()) return default(TInput);
            action(o);
            return o;
        }
    }
    
    internal class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _instances = new List<IDisposable>();

        public void Add(IDisposable instance)
        {
            _instances.Add(instance);
        }

        public void Dispose()
        {
            foreach (var instance in _instances)
            {
                instance.Dispose();
            }
        }
    }

    internal class DisposeCallback : IDisposable
    {
        readonly Action _disposeCallback;

        public DisposeCallback(Action disposeCallback)
        {
            if (disposeCallback == null) throw new ArgumentNullException("disposeCallback");

            _disposeCallback = disposeCallback;
        }

        public void Dispose()
        {
            _disposeCallback();
        }
    }

    // http://madskristensen.net/post/a-shorter-and-url-friendly-guid
    internal static class GuidEncoder
    {
        public static string Encode(string guidText)
        {
            var guid = new Guid(guidText);
            return Encode(guid);
        }

        public static string Encode(Guid guid)
        {
            var enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public static Guid Decode(string encoded)
        {
            encoded = encoded.Replace("_", "/");
            encoded = encoded.Replace("-", "+");
            var buffer = Convert.FromBase64String(encoded + "==");
            return new Guid(buffer);
        }
    }

    internal class DeterministicGuid
    {
        public Guid NameSpace;
        private readonly byte[] _namespaceBytes;

        public DeterministicGuid(Guid guidNameSpace)
        {
            NameSpace = guidNameSpace;
            _namespaceBytes = guidNameSpace.ToByteArray();
            SwapByteOrder(_namespaceBytes);
        }

        public Guid Create(byte[] input)
        {
            byte[] hash;
            using (var algorithm = SHA1.Create())
            {
                algorithm.TransformBlock(_namespaceBytes, 0, _namespaceBytes.Length, null, 0);
                algorithm.TransformFinalBlock(input, 0, input.Length);
                hash = algorithm.Hash;
            }

            var newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }

        private static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        private static void SwapBytes(byte[] guid, int left, int right)
        {
            var temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }
    }

    internal static class ExtensionsToDeterministicGuid
    {
        public static Guid Create(this DeterministicGuid source, Guid input)
        {
            return source.Create(input.ToByteArray());
        }

        public static Guid Create(this DeterministicGuid source, string input)
        {
            return source.Create(Encoding.UTF8.GetBytes(input));
        }

        public static Guid Create(this DeterministicGuid source, int input)
        {
            return source.Create(BitConverter.GetBytes(input));
        }
    }
    
    /// <summary>
    /// Helper class to dispatch to handler instances, adapted from https://gist.github.com/danbarua/8a9387957f8a0d884f41
    /// </summary>
    internal class ConventionDispatcher
    {
        public enum Delivery
        {
            Optional,
            Required
        }

        private readonly string _methodName;

        private static readonly IDictionary<Type, IDictionary<Type, MethodInfo>> Cache = new ConcurrentDictionary<Type, IDictionary<Type, MethodInfo>>();

        private static readonly MethodInfo InternalPreserveStackTraceMethod = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

        public ConventionDispatcher(string methodName)
        {
            _methodName = methodName;
        }

        [DebuggerNonUserCode]
        public void Dispatch(object instance, object @event, Delivery delivery = Delivery.Required)
        {
            var instanceType = instance.GetType();

            MethodInfo info;
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
                if (delivery == Delivery.Optional) return;

                var exceptionMessage = string.Format("Failed to locate {0}.{1}({2})", instanceType.Name, _methodName, eventType.Name);
                throw new InvalidOperationException(exceptionMessage);
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

    internal static class ExtensionsToString
    {
        public static Guid ToGuid(this string source)
        {
            return Guid.Parse(source);
        }

        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (!char.IsUpper(input[0])) return input;

            var chars = input.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                var hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1])) break;

#if !(DOTNET || PORTABLE)
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
#else
                chars[i] = char.ToLowerInvariant(chars[i]);
#endif
            }

            return new string(chars);
        }
    }

    internal static class ExtensionsToDictionary
    {
        public static void CopyFrom<TKey, TValue>(this IDictionary<string, object> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var tuple in source)
            {
                var key = tuple.Key as string ?? tuple.Key.ToString();
                target[key] = tuple.Value;
            }
        }

        public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey)
        {
            TValue value;
            if (!dictionary.TryGetValue(oldKey, out value)) return false;

            dictionary.Remove(oldKey);
            dictionary[newKey] = value;

            return true;
        }
    }
}