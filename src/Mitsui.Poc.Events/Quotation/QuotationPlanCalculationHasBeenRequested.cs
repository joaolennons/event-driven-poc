using System;

namespace Mitsui.Poc.Events
{
    public class QuotationPlanCalculationHasBeenRequested : IEvent
    {
        public Guid QuotationId { get; set; }
    }
}
