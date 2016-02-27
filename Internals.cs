using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventSpike
{
    internal class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type eventType) : base($"No handler found for {eventType.Name}")
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


}