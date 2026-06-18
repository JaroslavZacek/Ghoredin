using Ghoredin.Application.Characters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghoredin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        #region Get

        [HttpGet]
        public async Task<IActionResult> GetMyCharacters()
        {
            var characters = await _characterService.GetMyCharactersAsync();

            return Ok(characters);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var character = await _characterService.GetByIdAsync(id);

            if (character == null)
                return NotFound();

            return Ok(character);
        }

        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterCommand command)
        {
            var character = await _characterService.CreateAsync(command);

            return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
        }

        #endregion
    }
}
