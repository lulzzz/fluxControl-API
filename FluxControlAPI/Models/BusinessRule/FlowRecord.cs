using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataModels;
using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class FlowRecord
    {
        public int Id { get; set; }
        public User RegistryClerk { get; set; }
        public Bus BusRegistered { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
    }
}
