using System;

namespace EventSpike.Common
{
    public class DisposeCallback :
        IDisposable
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
}
