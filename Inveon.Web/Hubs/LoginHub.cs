using Microsoft.AspNetCore.SignalR;

namespace Inveon.Web.Hubs
{
    public class LoginHub : Hub
    {

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
