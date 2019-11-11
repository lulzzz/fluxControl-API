using FluxControlAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.SystemModels;
using FluxControlAPI.Models.BusinessRule;

namespace FluxControlAPI.Models
{
    public static class SystemNotifier
    {
        private static IHubContext<HistoricHub> _hub;
        
        public static void Init(IHubContext<HistoricHub> hub)
        {
            if (_hub == null)
                _hub = hub;
        }

        public static Task NotifyBysArrivedAsync(Bus bus)
        {
            return _hub.Clients.All.SendAsync("VehicleArrived", bus);
        }
    }
}
