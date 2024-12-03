using Microsoft.AspNetCore.SignalR;

namespace QuantumBasedQuantTrading.APIEndpoint.Services
{
    public class SignalRHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Conected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.Caller.SendAsync("Disconnected", Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
