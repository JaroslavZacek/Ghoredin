using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ghoredin.Server.Hubs
{
    [Authorize]
    public class CampaignHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinCampaignGroup(string campaignId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(campaignId));
        }

        public async Task LeaveCampaignGroup(string campaignId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(campaignId));
        }

        private static string GroupName(string campaignId) => $"campaign-{campaignId}";
    }
}
