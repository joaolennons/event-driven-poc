using EventStore;
using Mitsui.Poc.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStore
{
    public interface IEventStore
    {
        Task<bool> AppendToStreamAsync(string streamId, int expectedVersion, IEnumerable<IEvent> events);
        Task<EventStream> LoadStreamAsync(Guid streamId);
    }
}
