using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Occurence
    {
        public int Id { get; set; }
        public string Justification { get; set; }
        public List<Bus> Buses { get; set; }
    }
}
