using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Voice;

namespace Ghoredin.Infrastructure.Voice
{
    public class InMemoryVoiceStateService : IVoiceStateService
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, VoiceParticipantState>> _rooms = new();

        private readonly ConcurrentDictionary<string, (Guid CampaignId, string UserId)> _connections = new();

        public List<VoiceParticipantState> Join(Guid campaignId, string userId, string connectionId)
        {
            var room = _rooms.GetOrAdd(campaignId, _ => new ConcurrentDictionary<string, VoiceParticipantState>());

            var existingParticipant = room.Values.ToList();

            room[userId] = new VoiceParticipantState
            {
                UserId = userId
            };

            _connections[connectionId] = (campaignId, userId);

            return existingParticipant;
        }

        public void Leave(Guid campaignId, string userId)
        {
            if (_rooms.TryGetValue(campaignId, out var room))
                room.TryRemove(userId, out _);

            var toRemove = _connections
                .Where(kv => kv.Value.CampaignId == campaignId && kv.Value.UserId == userId)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var connId in toRemove)
                _connections.TryRemove(connId, out _);
        }

        public void SetSelfMute(Guid campaignId, string userId, bool muted)
        {
            if (_rooms.TryGetValue(campaignId, out var room) && room.TryGetValue(userId, out var state))
                state.IsSelfMuted = muted;
        }

        public void SetForceMute(Guid campaignId, string userId, bool muted)
        {
            if (_rooms.TryGetValue(campaignId, out var room) && room.TryGetValue(userId, out var state))
                state.IsForceMuted = muted;
        }

        public void SetSpeaking(Guid campaignId, string userId, bool speaking)
        {
            if (_rooms.TryGetValue(campaignId, out var room) && room.TryGetValue(userId, out var state))
                state.IsSpeaking = speaking;
        }

        public List<VoiceParticipantState> GetParticipants(Guid campaignId)
        {
            return _rooms.TryGetValue(campaignId, out var room) ? room.Values.ToList() : new List<VoiceParticipantState>();
        }

        public (Guid CampaignId, string UserId)? FindByConnection(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var info) ? info : null;
        }
    }
}
