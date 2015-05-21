using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using CommonDomain;
using Magnum.Reflection;

namespace EventSpike.ApprovalProcessor.CommonDomain.Automatonymous
{
    public abstract class EventSourcedAutomatonymousStateMachine<TInstance> :
        AutomatonymousStateMachine<TInstance>,
        ISaga
        where TInstance : class, new()
    {
        private readonly ICollection<object> _uncommitted = new LinkedList<object>();
        private readonly ICollection<object> _undispatched = new LinkedList<object>();
        private readonly InstanceLift<AutomatonymousStateMachine<TInstance>> _instanceLift;

        protected EventSourcedAutomatonymousStateMachine()
        {
            _instanceLift = this.CreateInstanceLift(new TInstance());
        }

        public void Transition(object message)
        {
            this.FastInvoke(new[] { message.GetType() }, _ => _.TransitionGeneric(default(object)), message);
            _uncommitted.Add(message);
            Version++;
        }

        private void TransitionGeneric<TMessage>(TMessage message)
        {
            foreach (var @event in Events.OfType<Event<TMessage>>())
            {
                _instanceLift.Raise(@event, message);
            }
        }

        protected void Dispatch(object message)
        {
            _undispatched.Add(message);
        }

        ICollection ISaga.GetUncommittedEvents()
        {
            return _uncommitted as ICollection;
        }

        void ISaga.ClearUncommittedEvents()
        {
            _uncommitted.Clear();
        }

        ICollection ISaga.GetUndispatchedMessages()
        {
            return _undispatched as ICollection;
        }

        void ISaga.ClearUndispatchedMessages()
        {
            _undispatched.Clear();
        }

        public string Id { get; protected set; }
        public int Version { get; private set; }
    }
}