using FluxControlAPI.Models.Datas.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime GenerationDate { get; set; }
        public decimal TaxConsidered { get; set; }
        public int IntervalMinutesConsidered { get; set; }
        public int CompanyDebtor { get; set; }
        public List<FlowRecord> Records { get; set; }
        public decimal TotalCost { get; set; }
    }
}
