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

        public CampaignService(ICampaignRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
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
    }
}
