using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ArduinoCommunication.Models;
using FluxControlAPI.Models;

namespace FluxControlAPI.Hubs
{
    public class HistoricHub : Hub
    {
        public async Task SendMessage(int status)
        {
            SerialCommunication.CommunicationStatusType serialCommuncation = (SerialCommunication.CommunicationStatusType) status;
            SerialCommunication.SendStatus(serialCommuncation);
            await Clients.All.SendAsync("ReceiveMessage", serialCommuncation);
        }

        public async Task VehicleArrived(string licensePlate)
        {
            await Clients.All.SendAsync("VehicleArrived", licensePlate);
        }
        
    }
}
