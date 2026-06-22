using Ghoredin.Application.Characters;
using Ghoredin.Server.Requests;
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

        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetCampaignCharacters(Guid campaignId)
        {
            try
            {
                var characters = await _characterService.GetCampaignCharactersAsync(campaignId);

                return Ok(characters);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Post

        [HttpPost("campaign/{campaignId:guid}")]
        public async Task<IActionResult> CreateInCampaign(Guid campaignId, [FromBody] CreateCharacterInCampaignRequest request)
        {
            try
            {
                var command = new CreateCharacterInCampaignCommand(campaignId, request.Name, request.SheetData ?? new());

                var character = await _characterService.CreateInCampaignAsync(command);

                return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
