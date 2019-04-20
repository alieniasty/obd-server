using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace OBDPI.Server.Hubs
{
    public class TelemetryHub : Hub
    {
        private readonly IHubContext<TelemetryHub> _context;

        public static HashSet<string> Connections { get; set; } = new HashSet<string>();

        public TelemetryHub(IHubContext<TelemetryHub> context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            Connections.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendAndForget(object data)
        {
            foreach (var connectionId in Connections)
            {
                await _context.Clients.Client(connectionId)
                    .SendAsync("Receive", data);
            }
            
        }
    }
}
