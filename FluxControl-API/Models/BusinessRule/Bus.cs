using FluxControlAPI.Models.Datas.BusinessRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.BusinessRule
{
    public class Bus
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int BusCompany { get; set; }
        public string LicensePlate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
