using Ghoredin.Application.Chat;

using Microsoft.AspNetCore.SignalR;

namespace Ghoredin.Server.Hubs
{
    public class SignalRChatNotifier : IChatNotifier
    {
        private readonly IHubContext<CampaignHub> _hubContext;

        public SignalRChatNotifier(IHubContext<CampaignHub> hubContext>)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMessageAsync(ChatMessageDto message, string authorUserId, string? whisperToUserId)
        {
            if (whisperToUserId is null)
            {
                await _hubContext.Clients
                    .Group($"campaign-{message.CampaignId}")
                    .SendAsync("ReceiveMessage", message);
            }
            else
            {
                await _hubContext.Clients
                    .Users(new[] { authorUserId, whisperToUserId })
                    .SendAsync("ReceiveMessage", message);
            }
        }
    }
}
