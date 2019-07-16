using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ScribrAPI.CentralHub
{
    public class PushNotification : Hub
    {
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }
    }
}
