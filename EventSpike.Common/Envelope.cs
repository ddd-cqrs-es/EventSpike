namespace EventSpike.Common
{
    public class Envelope<TMessage>
    {
        public readonly TMessage Message;
        public readonly MessageHeaders Headers;

        public Envelope(TMessage message, MessageHeader[] headers)
        {
            Message = message;
            Headers = new MessageHeaders(headers);
        }
    }

    public class Envelope
    {
        public readonly object Message;
        public readonly MessageHeaders Headers;

        public Envelope(object message, MessageHeader[] headers)
        {
            Message = message;
            Headers = new MessageHeaders(headers);
        }
    }
}
