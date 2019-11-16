using FluxControlAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.SystemModels;
using FluxControlAPI.Models.BusinessRule;
using Microsoft.AspNetCore.SignalR;

namespace FluxControlAPI.Models
{
    public class SystemNotifier
    {
        private IHubContext<HistoricHub> _hub;
        
        public SystemNotifier(IHubContext<HistoricHub> hub)
        {
            this._hub = hub;
        }

        public void VehicleActionAsync(FlowRecord record)
        {
            _hub.Clients.All.SendAsync("VehicleAction", record);
        }
    }
}
