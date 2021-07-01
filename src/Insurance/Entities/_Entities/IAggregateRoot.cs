using Mitsui.Poc.Events;
using System.Collections.Generic;

namespace Insurance
{
    public interface IAggregateRoot
    {
        List<IEvent> Changes { get; }

        void Rehydrate(IEnumerable<IEvent> events);
    }
}
