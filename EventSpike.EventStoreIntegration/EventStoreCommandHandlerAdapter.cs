using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventSpike.Messaging;
using EventSpike.Runtime;
using EventStore.ClientAPI;
using Magnum.Reflection;
using Newtonsoft.Json;

namespace EventSpike.EventStoreIntegration
{
    public class EventStoreCommandHandlerAdapter : ISystemInitializer
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSubscription _subscription;
        private readonly Type[] _messageTypes;
        private readonly IHandler _handler;
        private readonly ConventionDispatcher _dispatcher;

        public EventStoreCommandHandlerAdapter(IEventStoreConnection connection, EventStoreSubscription subscription, Type[] messageTypes, IHandler handler)
        {
            _connection = connection;
            _subscription = subscription;
            _messageTypes = messageTypes;
            _handler = handler;
            _dispatcher = new ConventionDispatcher("Handle");
        }
        
        public void Initialize()
        {
            _connection.ConnectToPersistentSubscription(_subscription.StreamName, _subscription.GroupName, OnEventAppeared);
        }

        private void OnEventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            var eventJson = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

            var eventType = _messageTypes.FirstOrDefault(@type => string.Equals(@type.Name, resolvedEvent.Event.EventType));

            if (eventType == null)
            {
                throw new MessageTypeMissing(resolvedEvent.Event.EventType);
            }

            var @event = JsonConvert.DeserializeObject(eventJson, eventType);
            
            var metadataJson = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
            var metadata = (Dictionary<string, string>)JsonConvert.DeserializeObject(metadataJson, typeof (Dictionary<string, string>));

            metadata.ChangeKey("$causationId", Constants.CausationIdKey);

            metadata.Add(Constants.StreamCheckpointTokenKey, resolvedEvent.Event.EventNumber.ToString());

            var envelope = this.FastInvoke(new[] {eventType}, x => x.CreateEnvelope(default(object), default(Dictionary<string, string>)), @event, metadata);

            _dispatcher.Dispatch(_handler, envelope);
        }

        private Envelope<TEvent> CreateEnvelope<TEvent>(TEvent @event, Dictionary<string, string> metadata)
        {
            return new Envelope<TEvent>(@event, metadata.ToMessageHeaders());
        }
    }
}
