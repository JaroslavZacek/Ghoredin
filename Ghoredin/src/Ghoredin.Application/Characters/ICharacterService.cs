using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public interface ICharacterService
    {
        Task<Character> CreateAsync(CreateCharacterCommand command);
        Task<List<Character>> GetMyCharactersAsync();
        Task<Character> GetByIdAsync(Guid id);
    }
}
