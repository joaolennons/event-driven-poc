using Mitsui.Poc.Events;
using System;
using System.Collections.Generic;

namespace Insurance
{
    public abstract class _AggregateRoot : IAggregateRoot, IEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public int Version { get; private set; }
        public List<IEvent> Changes { get; private set; }
        public _AggregateRoot()
        {
            Changes = new List<IEvent>();
        }

        public _AggregateRoot(IEnumerable<IEvent> events)
        {
            Rehydrate(events);
        }

        public void Rehydrate(IEnumerable<IEvent> events)
        {
            Changes = new List<IEvent>();

            foreach (var @event in events)
            {
                Mutate(@event);
                Version += 1;
            }
        }

        protected void Apply(IEvent @event)
        {
            Changes.Add(@event);
            Mutate(@event);
        }

        private void Mutate(IEvent @event)
        {
            ((dynamic)this).When((dynamic)@event);
        }
    }
}
