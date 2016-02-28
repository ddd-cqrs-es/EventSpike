using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSpike.Messaging
{
    public class MessageHeaders
    {
        public readonly ICollection<MessageHeader> Headers;
        public static readonly MessageHeaders Empty = new MessageHeaders(new MessageHeader[0]);

        public MessageHeaders(MessageHeader[] headers)
        {
            if (headers == null)
            {
                Headers = new MessageHeader[0];
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
}