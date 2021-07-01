using System;
using System.Collections.Generic;

namespace Mitsui.Poc.Http.InputDto
{
    public class QuotationInput
    {
        public Guid QuotationId { get; set; }
        public string Identity { get; set; }
        public string ZipCode { get; set; }
        public bool MainDriver { get; set; }
        public string Chassis { get; set; }
        public string LicensePlate { get; set; }
        public List<dynamic> Questions { get; set; }
    }
}
