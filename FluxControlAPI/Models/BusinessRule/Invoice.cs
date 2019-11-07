using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Invoice
    {
        public Company Debtor { get; set; }
        public decimal TotalCost { get; set; }
    }
}
