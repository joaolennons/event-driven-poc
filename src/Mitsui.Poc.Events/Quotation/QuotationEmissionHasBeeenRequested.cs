using System;

namespace Mitsui.Poc.Events
{
    public class QuotationEmissionHasBeeenRequested : IEvent
    {
        public Guid QuotationId { get; set; }
    }
}
