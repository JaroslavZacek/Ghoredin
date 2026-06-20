using Ghoredin.Application.Characters;
using Ghoredin.Domain.Characters;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.Persistence.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly AppDbContext _context;

        public CharacterRepository(AppDbContext contexr)
        {
            _context = contexr;
        }

        /// <summary>
        /// Vrátí entitu postavy podle jejího identifikátoru (GUID).
        /// </summary>
        /// <param name="id">Identifikátor postavy (GUID).</param>
        /// <returns>Asynchronní <see cref="Task{Character}"/> vracející nalezenou instanci <see cref="Character"/> nebo <c>null</c>,
        /// pokud žádná entita s daným id neexistuje.
        /// </returns>
        public async Task<Character> GetByIdAsync(Guid id)
        {
            return await _context.Characters.FindAsync(id);
        }

        /// <summary>
        /// Asynchronně načte všechny entity typu <see cref="Character"/>, které vlastní uživatel se zadaným ID.
        /// </summary>
        /// <param name="ownerUserId">Identifikátor vlastníka postav (např. uživatelské Id). Očekává se neprázdný řetězec.</param>
        /// <returns>Seznam <see cref="Character"/> entit přiřazených zadanému uživateli. Pokud nejsou nalezeny žádné záznamy, metoda vrátí prázdný seznam.</returns>
        public async Task<List<Character>> GetByOwnerAsync(string ownerUserId)
        {
            return await _context.Characters
                .Where(c => c.OwnerUserId == ownerUserId)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronně načte všechny entity typu <see cref="Character"/>, které patří do kampaně se zadaným identifikátorem.
        /// </summary>
        /// <param name="campaignId">Identifikátor kampaně (<see cref="Guid"/>), jehož postavy se mají načíst.</param>
        /// <returns>Asynchronní <see cref="Task{List{Character}}"/> vracející seznam nalezených instancí <see cref="Character"/>.
        /// Pokud pro danou kampaň nejsou žádné postavy, vrátí se prázdný seznam.</returns>
        public async Task<List<Character>> GetByCampaignAsync(Guid campaignId)
        {
            return await _context.Characters
                .Where(c => c.CampaignId == campaignId)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronně přidá novou instanci <c>Character</c> do DbSetu <c>Characters</c> v kontextu.
        /// </summary>
        /// <param name="character">Objekt <c>Character</c>, který má být přidán. Nesmí být <c>null</c>.</param>
        /// <returns>Asynchronický <c>Task</c>. Dokončení znamená, že entita byla úspěšně přidána do kontextu.</returns>
        public async Task AddAsync(Character character)
        {
            await _context.Characters.AddAsync(character);
        }

        /// <summary>
        /// Asynchronně uloží všechny neuložené změny aktuálního DbContextu do databáze.
        /// </summary>
        /// <returns>Asynchronní <see cref="Task"/>, která se dokončí po úspěšném uložení změn.</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
