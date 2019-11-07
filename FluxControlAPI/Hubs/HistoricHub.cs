using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FluxControlAPI.Models;
using FluxControlAPI.Models.BusinessRule;

namespace FluxControlAPI.Hubs
{
    public class HistoricHub : Hub
    {
        public async Task SendMessage(int message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task VehicleArrived(Bus bus)
        {
            await Clients.All.SendAsync("VehicleArrived", bus);
        }
        
    }
}
