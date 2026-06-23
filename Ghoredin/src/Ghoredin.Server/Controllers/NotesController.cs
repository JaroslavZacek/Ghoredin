using Ghoredin.Application.Notes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ghoredin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        #region Get

        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetCampaignNotes(Guid campaignId)
        {
            try
            {
                var notes = await _noteService.GetCampaignNotesAsync(campaignId);

                return Ok(notes);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoteCommand command)
        {
            try
            {
                var note = await _noteService.CreateAsync(command);

                return Ok(note);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Put

        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteCommand request)
        {
            try
            {
                var command = new UpdateNoteCommand(
                    id,
                    request.Title,
                    request.Content,
                    request.PlayerFacingContent,
                    request.Visibility);

                var note = await _noteService.UpdateAsync(command);

                return Ok(note);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
