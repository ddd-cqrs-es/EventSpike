using System.Collections.Generic;

namespace NEventStore.Spike.Common
{
    public interface IEnvelope
    {
        IDictionary<string, object> Headers { get; } 
        object Body { get; }
    }

    public interface IEnvelope<out TBody> :
        IEnvelope
    {
        new TBody Body { get; }
    }

    public class Envelope<TBody>
        : IEnvelope<TBody>
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

        public Envelope<TBody> AddHeader<THeader>(THeader header)
        {
            Headers.Store(header);

            return this;
        } 

        public IDictionary<string, object> Headers { get; private set; }

        public TBody Body { get; private set; }

        object IEnvelope.Body { get { return Body; } }
    }
}
