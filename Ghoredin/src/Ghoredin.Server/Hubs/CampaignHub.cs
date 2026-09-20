using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Voice;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ghoredin.Server.Hubs
{
    [Authorize]
    public class CampaignHub : Hub
    {
        private readonly IVoiceStateService _voiceStateService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;

        public CampaignHub(
            IVoiceStateService voiceStateService,
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService)
        {
            _voiceStateService = voiceStateService;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
        }

        #region Chat

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        //Upraveno pro používání i ve voice
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var found = _voiceStateService.FindByConnection(Context.ConnectionId);

            if (found is not null)
            {
                var (campaignId, userId) = found.Value;

                _voiceStateService.Leave(campaignId, userId);

                await Clients.OthersInGroup(GroupName(campaignId.ToString()))
                    .SendAsync("VoiceParticipantLeft", new { userId });
            }

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

        #endregion

        #region Voice

        public async Task JoinVoice(string campaignId)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            var existingParticipants = _voiceStateService.Join(Guid.Parse(campaignId), userId, Context.ConnectionId);

            await Clients.Caller.SendAsync("VoiceParticipantsSnapshot", existingParticipants);

            await Clients.OthersInGroup(GroupName(campaignId))
                .SendAsync("VoiceParticipantJoined", new { userId });
        }

        public async Task LeaveVoice(string campaignId)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            _voiceStateService.Leave(Guid.Parse(campaignId), userId);

            await Clients.OthersInGroup(GroupName(campaignId))
                .SendAsync("VoiceParticipantLeft", new { userId });
        }

        public async Task SendVoiceSignal(string campaignId, string targetUserId, string signalType, string payload)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            await Clients.User(targetUserId).SendAsync("ReceiveVoiceSignal", new
            {
                fromUserId = userId,
                signalType,
                payload
            });
        }

        public async Task SetSelfMute(string campaignId, bool muted)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            _voiceStateService.SetSelfMute(Guid.Parse(campaignId), userId, muted);

            await Clients.Group(GroupName(campaignId))
                .SendAsync("VoiceMuteChanged", new { userId, isSelfMuted = muted });
        }

        public async Task SetForceMute(string campaignId, string targetUserId, bool muted)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            var campaign = await _campaignRepository.GetByIdAsync(Guid.Parse(campaignId))
                ?? throw new HubException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new HubException("Jen PJ může umlčet ostatní hráče.");

            _voiceStateService.SetForceMute(Guid.Parse(campaignId), targetUserId, muted);

            await Clients.Group(GroupName(campaignId))
                .SendAsync("VoiceForceMuteChanged", new { userId = targetUserId, isForceMuted = muted });
        }

        public async Task SetSpeaking(string campaignId, bool speaking)
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Nejsi přihlášený.");

            _voiceStateService.SetSpeaking(Guid.Parse(campaignId), userId, speaking);

            await Clients.Group(GroupName(campaignId))
                .SendAsync("VoiceSpeakingChanged", new { userId, isSpeaking = speaking });
        }

        #endregion

    }
}
