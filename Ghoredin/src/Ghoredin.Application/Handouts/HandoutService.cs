using System;
using System.Collections.Generic;
using System.Text;

using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Handouts;

namespace Ghoredin.Application.Handouts
{
    public class HandoutService : IHandoutService
    {
        private readonly IHandoutRepository _handoutRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;
        private readonly ICurrentUserService _currentUserService;

        public HandoutService(
            IHandoutRepository handoutRepository,
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _handoutRepository = handoutRepository;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
            _currentUserService = currentUserService;
        }

        public async Task<HandoutDto> CreateAsync(CreateHandoutCommand command)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(command.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException("Jen PJ může vytvářet listiny.");

            var handout = new Handout
            {
                Id = Guid.NewGuid(),
                CampaignId = command.CampaignId,
                Title = command.Title,
                Content = command.Content,
                ShareMode = command.ShareMode,
                IsShared = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _handoutRepository.AddAsync(handout);
            await _handoutRepository.SaveChangesAsync();

            return handout.ToDto(isGameMaster: true);
        }

        public async Task<HandoutDto> UpdateAsync(UpdateHandoutCommand command)
        {
            var (handout, campaign, userId) = await LoadForGmAsync(command.Id, "upravovat");

            handout.Title = command.Title;
            handout.Content = command.Content;
            handout.UpdatedAt = DateTime.UtcNow;

            await _handoutRepository.SaveChangesAsync();

            return handout.ToDto(isGameMaster: true);
        }


        //----------------------------------------------------------------------------
        //-----------------------------Privátní metody--------------------------------
        //----------------------------------------------------------------------------

        private async Task<(Handout handout, Domain.Campaigns.Campaign campaign, string userId)> LoadForGmAsync(
            Guid handoutId, string actionVerb)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var handout = await _handoutRepository.GetByIdAsync(handoutId)
                ?? throw new InvalidOperationException("Listina neexistuje.");

            var campaign = await _campaignRepository.GetByIdAsync(handout.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException($"Jen PJ může {actionVerb} listiny.");

            return (handout, campaign, userId);
        }
    }
}
