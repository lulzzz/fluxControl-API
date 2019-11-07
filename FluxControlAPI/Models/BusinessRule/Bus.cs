using FluxControlAPI.Models.DataModels.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Bus
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int BusCompany { get; set; }
        public string LicensePlate { get; set; }
    }
}
