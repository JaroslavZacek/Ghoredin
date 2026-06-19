using Ghoredin.Application.Users;
using Ghoredin.Domain.Campaigns;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Campaigns
{
    public class CampaignService: ICampaignService
    {
        private readonly ICampaignRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;

        public CampaignService(ICampaignRepository repository, ICurrentUserService currentUser, ICampaignAuthorizationService campaignAuthorizationService)
        {
            _repository = repository;
            _currentUser = currentUser;
            _campaignAuthorizationService = campaignAuthorizationService;
        }

        public async Task<CampaignDto> CreateAsync(CreateCampaignCommand command)
        {
            var userId = _currentUser.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = new Campaign
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                GameSystemId = command.GameSystemId,
                MaxPlayers = command.MaxPlayers
            };

            campaign.Members.Add(new CampaignMember
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                UserId = userId,
                Role = CampaignRole.GameMaster
            });

            await _repository.AddAsync(campaign);
            await _repository.SaveChangesAsync();

            return campaign.ToDto();
        }

        public async Task<List<CampaignDto>> GetMyCampaignsAsync()
        {
            var userId = _currentUser.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaigns = await _repository.GetByMemberAsync(userId);

            return campaigns.Select(c => c.ToDto()).ToList();
        }

        public async Task<CampaignDto?> GetByIdAsync(Guid id)
        {
            var campaign = await _repository.GetByIdAsync(id);

            return campaign?.ToDto();
        }

        public async Task JoinAsync(JoinCampaignCommand command)
        {
            var userId = _currentUser.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _repository.GetByIdAsync(command.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (_campaignAuthorizationService.IsMember(campaign, userId))
                throw new InvalidOperationException("Už jsi členem tohoto dobrodružství.");

            var playerCount = campaign.Members.Count(m => m.Role == CampaignRole.Player);

            if (campaign.MaxPlayers.HasValue && playerCount >= campaign.MaxPlayers.Value)
                throw new InvalidOperationException("Dobrodružství už je plné dobrodruhů.");

            campaign.Members.Add(new CampaignMember
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                UserId = userId,
                Role = CampaignRole.Player,
                CharacterId = command.CharacterId
            });

            await _repository.SaveChangesAsync();
        }

        public async Task<List<CampaignDto>> GetAvailableCampaignsAsync()
        {
            var userId = _currentUser.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var allCampaigns = await _repository.GetAllAsync();

            var available = allCampaigns.Where(c =>
                !_campaignAuthorizationService.IsMember(c, userId) &&
                (!c.MaxPlayers.HasValue ||
                c.Members.Count(m => m.Role == CampaignRole.Player) < c.MaxPlayers.Value));

            return available.Select(c => c.ToDto()).ToList();
        }
    }
}
