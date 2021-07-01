using Newtonsoft.Json;
using System;

namespace Insurance.Query
{
    public class TransactionProjection
    {
        [JsonProperty("id")]
        public Guid QuotationIdentifier { get; set; }
        public string Status { get; set; }
        public string InsuredName { get; set; }
        public string InsuredEmail { get; set; }
        public string Car { get; set; }
        public string LicensePlate { get; set; }
        public string Plan { get; set; }
        public string Value { get; set; }
    }
}
