using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Journal;

namespace Ghoredin.Application.Journal
{
    public class JournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;
        private readonly ICurrentUserService _currentUserService;

        public JournalService(
            IJournalRepository journalRepository,
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _journalRepository = journalRepository;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
            _currentUserService = currentUserService;
        }

        public async Task<JournalEntryDto> SaveAsync(SaveJournalCommand command)
        {
            var userId = await EnsureMemberAsync(command.CampaignId);

            var entry = await _journalRepository.GetByOwnerAndCampaignAsync(command.CampaignId, userId);

            if (entry is null)
            {
                entry = new JournalEntry
                {
                    Id = Guid.NewGuid(),
                    CampaignId = command.CampaignId,
                    OwnerUserId = userId,
                    Content = command.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _journalRepository.AddAsync(entry);
            }
            else
            {
                entry.Content = command.Content;
                entry.UpdatedAt = DateTime.UtcNow;
            }

            await _journalRepository.SaveChangesAsync();

            return entry.ToDto();
        }

        public async Task<JournalEntryDto> GetMyEntryAsync(Guid campaignId)
        { 
            var userId = await EnsureMemberAsync(campaignId);

            var entry = await _journalRepository.GetByOwnerAndCampaignAsync(campaignId, userId);

            return entry?.ToDto();
        }

        // -----------------------------------------------------------------------------------
        // ------------------------------Privátní metody--------------------------------------
        // -----------------------------------------------------------------------------------

        private async Task<string> EnsureMemberAsync(Guid campaignId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsMember(campaign, userId))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            return userId;
        }
    }
}
