using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Voice
{
    public interface IVoiceStateService
    {
        List<VoiceParticipantState> Join(Guid campaignId, string userId, string connectionId);

        void Leave(Guid campaignId, string userId);

        void SetSelfMute(Guid campaignId, string userId, bool muted);
        void SetForceMute(Guid campaignId, string userId, bool muted);
        void SetSpeaking(Guid campaignId, string userId, bool speaking);

        List<VoiceParticipantState> GetParticipants(Guid campaignId);

        (Guid CampaignId, string UserId)? FindByConnection(string connectionId);
    }
}
