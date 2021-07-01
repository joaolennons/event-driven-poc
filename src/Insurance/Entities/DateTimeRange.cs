using Newtonsoft.Json;
using System;

namespace Insurance
{
    public class DateTimeRange
    {
        [JsonConstructor]
        private DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        [JsonProperty]
        public DateTime Start { get; private set; }
        [JsonProperty] 
        public DateTime End { get; private set; }
        public bool IsValid() => Start > DateTime.Now && DateTime.Now < End;
        public double DifferenceInYears() => (End.Date - Start.Date).TotalDays / 365;
        public static DateTimeRange Create(DateTime start, DateTime end)
            => new DateTimeRange(start, end);

        internal static DateTimeRange Create(object vigencyStart, DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
