using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FluxControlAPI.Models;
using FluxControlAPI.Models.BusinessRule;
using Microsoft.AspNet.SignalR.Hubs;

namespace FluxControlAPI.Hubs
{
    public class HistoricHub : Hub
    {
        public async Task Info(string message)
        {
            await Clients.All.SendAsync("Info", message);
        }

        public async Task VehicleAction(FlowRecord record)
        {
            await Clients.All.SendAsync("VehicleAction", record);
        }

        public async Task OccurrenceWarning(Occurrence occurrence)
        {
            await Clients.All.SendAsync("OccurrenceWarning", occurrence);
        }

    }
}
