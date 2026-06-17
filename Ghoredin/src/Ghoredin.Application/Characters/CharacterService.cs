using Ghoredin.Application.Users;
using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public class CharacterService: ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICurrentUserService _currentUserService;

        public CharacterService(ICharacterRepository repository, ICurrentUserService currentUserService)
        {
            _characterRepository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<Character> CreateAsync(CreateCharacterCommand command)
        {
            var character = new Character
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                GameSystemId = command.GameSystemId,
                SheetData = command.SheetData,
                OwnerUserId = _currentUserService.UserId
                    ?? throw new InvalidOperationException("Není přihlášený uživatel.")
            };

            await _characterRepository.AddAsync(character);

            await _characterRepository.SaveChangesAsync();

            return character;
        }

        public async Task<List<Character>> GetMyCharactersAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new InvalidOperationException("Není přihlášený uživatel.");

            return await _characterRepository.GetByOwnerAsync(userId);
        }

        public async Task<Character> GetByIdAsync(Guid id)
        {
            return await _characterRepository.GetByIdAsync(id);
        }
    }
}
