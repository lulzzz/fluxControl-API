using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluxControlAPI.Models.APIs.OpenALPR.Models
{
    public class OpenALPRResult
    {
        public string Plate { get; set; }
        public double Confidence { get; set; }
    }

    public class OpenALPRResponse
    {

        public bool Error { get; set; }
        public OpenALPRResult[] Results { get; set; }

    }
}
