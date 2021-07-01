using Mitsui.Poc.Events;
using System;
using System.Collections.Generic;

namespace Insurance
{
    public abstract class _Quotation : _AggregateRoot
    {
        public _Quotation() : base()
        {
            CreatedAt = DateTime.Now;
        }

        public _Quotation(IEnumerable<IEvent> events) : base(events)
        {
        }

        public Plan Plan { get; protected set; }
        public RiskCriteria RiskCriteria { get; protected set; }
        public string Status { get; protected set; }
    }
}
