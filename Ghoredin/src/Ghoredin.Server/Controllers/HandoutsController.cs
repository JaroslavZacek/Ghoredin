using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ghoredin.Application.Handouts;
using Ghoredin.Server.Requests;

namespace Ghoredin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HandoutsController : Controller
    {
        private readonly IHandoutService _handoutService;

        public HandoutsController(IHandoutService handoutService)
        {
            _handoutService = handoutService;
        }

        #region Get

        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetForCampaign(Guid campaignId)
        {
            try
            {
                var handouts = await _handoutService.GetVisibleForCampaignAsync(campaignId);

                return Ok(handouts);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHandoutRequest request)
        {
            try
            {
                var command = new CreateHandoutCommand(
                    request.CampaignId,
                    request.Title,
                    request.Content,
                    request.ContentType,
                    request.ShareMode);

                var handout = await _handoutService.CreateAsync(command);

                return Ok(handout);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id:guid}/share")]
        public async Task<IActionResult> Share(Guid id)
        {
            try
            {
                await _handoutService.ShareAsync(id);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id:guid}/unshare")]
        public async Task<IActionResult> Unshare(Guid id)
        {
            try
            {
                await _handoutService.UnshareAsync(id);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion

        #region Put
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHandoutRequest request)
        {
            try
            {
                var command = new UpdateHandoutCommand(
                    id,
                    request.Title,
                    request.Content);

                var handout = await _handoutService.UpdateAsync(command);

                return Ok(handout);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
