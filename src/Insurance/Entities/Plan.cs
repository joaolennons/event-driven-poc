namespace Insurance
{
    public class Plan
    {
        public Plan(DateTimeRange vigency, decimal value)
        {
            Vigency = vigency;
            Value = value;
        }

        public DateTimeRange Vigency { get; private set; }
        public decimal Value { get; private set; }
        public bool IsYearly() => Vigency.DifferenceInYears() == 1;
    }
}
