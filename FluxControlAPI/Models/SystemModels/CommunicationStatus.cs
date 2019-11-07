using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArduinoCommunication.Models.SerialCommunication;

namespace FluxControlAPI.Models.SystemModels
{
    public class CommunicationStatus
    {
        public CommunicationStatusType StatusCode { get; set; }
        public string Data { get; set; }
    }
}
