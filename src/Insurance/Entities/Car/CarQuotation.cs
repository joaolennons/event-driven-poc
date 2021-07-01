using Mitsui.Poc.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Insurance.Car
{
    public class CarQuotation : _Quotation
    {
        public Insured Insured { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public bool IsValid() => true;
        
        public CarQuotation(string identity, string zipCode, bool mainDriver, string chassis, string licensePlate, List<dynamic> questions) : base()
        {
            Apply(new CarQuotationDraftHasBeenRegistered
            {
                QuotationId = Guid.NewGuid(),
                Identity = identity,
                ZipCode = zipCode,
                MainDriver = mainDriver,
                Chassis = chassis,
                LicensePlate = licensePlate,
                Questions = questions
            });
        }

        public CarQuotation(IEnumerable<IEvent> events) : base(events)
        {
            
        }

        public CarQuotation()
        {

        }

        public void UpdateDraft(string identity, string zipCode, bool mainDriver, string chassis, string licensePlate, List<dynamic> questions)
        {
            Apply(new CarQuotationDraftHasBeenUpdated
            {
                Identity = identity,
                ZipCode = zipCode,
                MainDriver = mainDriver,
                Chassis = chassis,
                LicensePlate = licensePlate,
                Questions = questions
            });
        }

        public bool RequestEmission()
        {
            if (Status == "Emited")
                return false; 

            Apply(new QuotationEmissionHasBeeenRequested());
            
            return true; 
        }

        public void Emit()
        {
            Apply(new QuotationHasBeenEmited());
        }

        public void ReportError(string source, string reason)
        {
            Apply(new IntegrationErrorHasBeenRaised { Source = source, Reason = reason });
        }

        public void SuggestPlan(decimal value)
        {
            Apply(new QuotationPlanCalculationIsDone { Value = value });
        }

        public void Calculate()
        {
            var @event = new QuotationPlanCalculationHasBeenRequested();
            Apply(@event);
        }

        internal void When(CarQuotationDraftHasBeenRegistered @event)
        {
            Id = @event.QuotationId;

            Status = "Draft";

            Insured = new Insured(@event.Identity, @event.ZipCode, @event.MainDriver);

            Vehicle = new Vehicle(@event.Chassis, @event.LicensePlate);

            RiskCriteria = new RiskCriteria(@event.Questions.Select(question => (Question)question)); 
        }

        internal void When(CarQuotationDraftHasBeenUpdated @event)
        {
            @event.QuotationId = Id;

            Insured = new Insured(@event.Identity, @event.ZipCode, @event.MainDriver);

            Vehicle = new Vehicle(@event.Chassis, @event.LicensePlate);

            @event.Questions.ForEach(question => RiskCriteria.AddCriteria((Question)question));
        }

        internal void When(QuotationPlanCalculationHasBeenRequested @event)
        {
            @event.QuotationId = Id;
            Status = "Draft";
        }

        internal void When(QuotationPlanCalculationIsDone @event)
        {
            if (Status == "Draft")
            {
                Plan = new Plan(DateTimeRange.Create(@event.VigencyStart, DateTime.Now.Date.AddYears(1)), @event.Value);
                Status = "Calculated";
            }
        }

        internal void When(IntegrationErrorHasBeenRaised @event)
        {
            
        }

        internal void When(QuotationProposalHasBeenRequested @event)
        {
            if (Plan.Value > 0)
                Status = "Proposal";
        }

        internal void When(CarInspectionHasBeenRequested @event)
        {
            if (Status == "Proposal")
                Status = "Inspection";
        }

        internal void When(QuotationEmissionHasBeeenRequested @event)
        {
            @event.QuotationId = Id;

            if (Status == "Inspection")
                Status = "Emission";

            Console.WriteLine("Prepare Emission Integration");
        }

        internal void When(QuotationHasBeenEmited @event)
        {
            Status = "Emited";
        }
    }
}
