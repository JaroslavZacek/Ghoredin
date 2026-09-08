using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Ghoredin.Application.Journal;
using Ghoredin.Server.Requests;

namespace Ghoredin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;

        public JournalController(IJournalService journalService)
        {
            _journalService = journalService;
        }

        #region Get

        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetMine(Guid campaignId)
        {
            try
            {
                var entry = await _journalService.GetMyEntryAsync(campaignId);

                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Put

        public async Task<IActionResult> Save(Guid campaignId, [FromBody] SaveJournalRequest request)
        {
            try
            {
                var command = new SaveJournalCommand(campaignId, request.Content);
                var entry = await _journalService.SaveAsync(command);

                return Ok(entry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
