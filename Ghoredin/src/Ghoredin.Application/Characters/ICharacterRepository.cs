using Ghoredin.Domain.Characters;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Characters
{
    public interface ICharacterRepository
    {
        Task<Character> GetByIdAsync(Guid id);
        Task<List<Character>> GetByOwnerAsync(string ownerUserId);
        Task AddAsync(Character character);
        Task SaveChangesAsync();
    }
}
