using System;

namespace Mitsui.Poc.Events
{
    public class QuotationPlanCalculationIsDone : IEvent
    {
        public DateTime VigencyStart { get; set; } = DateTime.Now.Date;
        public decimal Value { get; set; } = 1500m;
    }
}
