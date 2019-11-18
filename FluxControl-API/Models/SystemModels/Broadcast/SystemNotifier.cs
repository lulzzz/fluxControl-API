using FluxControlAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using Microsoft.AspNetCore.SignalR;
using FluxControlAPI.Models.SystemModels.Broadcast.Models;

namespace FluxControlAPI.Models.SystemModels.Broadcast
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

        public void WarningAsync(SystemWarning warning)
        {
            _hub.Clients.All.SendAsync("Warning", warning);
        }
    }
}
