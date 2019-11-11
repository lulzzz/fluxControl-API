using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Rules
    {
        public int Id { get; set; }
        public decimal Tax { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
