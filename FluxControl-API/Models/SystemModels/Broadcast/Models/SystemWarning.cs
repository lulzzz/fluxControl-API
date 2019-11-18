using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.SystemModels.Broadcast.Models
{
    public class SystemWarning
    {
        public enum WarningType
        {
            NotRecognized = 0,
            NotRegistered = 1
        };

        public DateTime Moment { get; set; }
        public WarningType Type { get; set; }
        public string LicensePlate { get; set; }
        public string Message { get; set; }
    }
}
