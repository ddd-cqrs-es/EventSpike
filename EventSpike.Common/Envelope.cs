using System;
using System.Collections.Generic;
using System.Linq;

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

    public class MessageHeaders
    {
        public readonly ICollection<MessageHeader> Headers;
        public static readonly MessageHeaders Empty = new MessageHeaders(new MessageHeader[0]);

        public MessageHeaders(MessageHeader[] headers)
        {
            if (headers == null)
            {
                Headers = MessageHeader.Empty;
            }
            else
            {
                var copy = new MessageHeader[headers.Length];
                Array.Copy(headers, copy, headers.Length);
                Headers = copy;
            }
        }

        public string this[string name] { get { return GetHeader(name); } }

        public string GetHeader(string name)
        {
            return Headers.First(n => n.Key == name).Value;
        }

        public string GetHeader(string name, string defaultValue)
        {
            foreach (var attribute in Headers.Where(attribute => attribute.Key == name))
            {
                return attribute.Value;
            }

            return defaultValue;
        }
    }

    public struct MessageHeader
    {
        public readonly string Key;
        public readonly string Value;

        public static readonly MessageHeader[] Empty = new MessageHeader[0];

        public MessageHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
