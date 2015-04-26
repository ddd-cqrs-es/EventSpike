using System.Collections.Generic;

namespace EventSpike.Common
{
    // TODO Consider using an immutable dict/map (eg. Funq?)
    public class Envelope<TBody>
    {
        public static Envelope<TBody> Create(TBody body)
        {
            return new Envelope<TBody>(body);
        }

        public static Envelope<TBody> Create(IDictionary<string, object> headers, TBody body)
        {
            return new Envelope<TBody>(headers, body);
        }

        private Envelope(IDictionary<string, object> headers, TBody body)
        {
            Body = body;
            Headers = headers;
        }

        private Envelope(TBody body)
            : this(new Dictionary<string, object>(), body)
        {
        }

        public IDictionary<string, object> Headers { get; private set; }

        public TBody Body { get; private set; }
    }
}
