using System;
using System.Text;
using EventSpike.Common;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventSpike.EventStoreIntegration
{
    public class EventStoreCommandHandlerAdapter : ISystemInitializer
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSubscription _subscription;
        private readonly IHandler _handler;
        private readonly ConventionDispatcher _dispatcher;

        public EventStoreCommandHandlerAdapter(IEventStoreConnection connection, EventStoreSubscription subscription, IHandler handler)
        {
            _connection = connection;
            _subscription = subscription;
            _handler = handler;
            _dispatcher = new ConventionDispatcher("Handle");
        }
        
        public void Initialize()
        {
            _connection.ConnectToPersistentSubscription(_subscription.StreamName, _subscription.GroupName, OnEventAppeared);
        }

        private void OnEventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent resolvedEvent)
        {
            var jsonString = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

            var eventType = Type.GetType(resolvedEvent.Event.EventType);

            var @event = JsonConvert.DeserializeObject(jsonString, eventType);

            _dispatcher.Dispatch(_handler, @event);
        }
    }
}
