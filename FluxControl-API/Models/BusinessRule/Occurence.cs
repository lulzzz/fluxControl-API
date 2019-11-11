using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Occurence
    {
        public enum OccurrenceType 
        {
            Crash = 0,
            Mechanical = 1,
            PhysicalAccident = 2,
            Structural = 3
        };

        public int Id { get; set; }
        public string Justification { get; set; }
        public List<Bus> Buses { get; set; }
        public List<OccurrenceType> Occurrences { get; set; }
    }
}
