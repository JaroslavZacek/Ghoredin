using Ghoredin.Application.Campaigns;
using Ghoredin.Application.Users;
using Ghoredin.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Notes
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignAuthorizationService _campaignAuthorizationService;
        private readonly ICurrentUserService _currentUserService;

        public NoteService(
            INoteRepository noteRepository, 
            ICampaignRepository campaignRepository,
            ICampaignAuthorizationService campaignAuthorizationService,
            ICurrentUserService currentUserService)
        {
            _noteRepository = noteRepository;
            _campaignRepository = campaignRepository;
            _campaignAuthorizationService = campaignAuthorizationService;
            _currentUserService = currentUserService;
        }

        public async Task<NoteDto> CreateAsync(CreateNoteCommand command)
        {

            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(command.CamapignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException("Jen PJ může psát poznámky");

            var note = new CampaignNote
            {
                Id = Guid.NewGuid(),
                CampaignId = command.CamapignId,
                AuthorUserId = userId,
                Title = command.Title,
                Content = command.Content,
                PlayerFacingContent = command.PlayerFacingContent,
                Visibility = command.Visibility,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _noteRepository.AddAsync(note);
            await _noteRepository.SaveChangesAsync();

            return note.ToDto(isGameMaster: true);
        }

        public async Task<NoteDto> UpdateAsync(UpdateNoteCommand command)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var note = await _noteRepository.GetByIdAsync(command.Id)
                ?? throw new InvalidOperationException("Poznámka neexistuje.");

            var campaign = await _campaignRepository.GetByIdAsync(note.CampaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje");

            if (!_campaignAuthorizationService.IsGameMaster(campaign, userId))
                throw new InvalidOperationException("Jen PJ může upravovat poznámky.");

            note.Title = command.Title;
            note.Content = command.Content;
            note.PlayerFacingContent = command.PlayerFacingContent;
            note.Visibility = command.Visibility;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.SaveChangesAsync();

            return note.ToDto(isGameMaster: true);
        }

        public async Task<List<NoteDto>> GetCampaignNotesAsync(Guid campaignId)
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            var campaign = await _campaignRepository.GetByIdAsync(campaignId)
                ?? throw new InvalidOperationException("Dobrodružství neexistuje.");

            if (!_campaignAuthorizationService.IsMember(campaign, userId))
                throw new InvalidOperationException("Nejsi členem tohoto dobrodružství.");

            var isGm = _campaignAuthorizationService.IsGameMaster(campaign, userId);
            var notes = await _noteRepository.GetByCampaignAsync(campaignId);

            var visible = isGm
                ? notes
                : notes.Where(n => n.Visibility == NoteVisibility.SharedWithPlayers);

            return visible.Select(n => n.ToDto(isGm)).ToList();
        }
    }
}
