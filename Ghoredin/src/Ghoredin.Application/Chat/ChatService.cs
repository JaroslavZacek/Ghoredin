using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;

using Ghoredin.Domain.Chat;

namespace Ghoredin.Application.Chat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;
        private readonly ICurrentUserService _currentUserService;

        public ChatService (
            IChatRepository chatRepository,
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _chatRepository = chatRepository;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
            _currentUserService = currentUserService;
        }

        public async Task<ChatMessageDto> SendMessageAsync(SendMessageCommand command)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(command.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje");

            if (!_campaignAuthorizationService.IsMember(campaign, userId))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství");

            if (command.WhisperToUserId is not null && !_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException("Jen PJ může šeptat.");

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                CampaignId = command.CampaignId,
                AuthorUserId = userId,
                Content = command.Content,
                WhisperToUserId = command.WhisperToUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRepository.AddAsync(message);
            await _chatRepository.SaveChangesAsync();

            return message.ToDto();
        }

        public async Task<List<ChatMessageDto>> GetVisibleHistoryAsync(Guid campaignId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsMember(campaign, userId))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            var isGm = _campaignAuthorizationService.IsGameMaster(campaign, userId);

            var messages = await _chatRepository.GetByCampaignAsync(campaignId);

            var visible = messages.Where(m =>
                m.WhisperToUserId is null ||
                m.AuthorUserId == userId ||
                m.WhisperToUserId == userId);

            return visible.Select(m => m.ToDto()).ToList();
        }
    }
}
