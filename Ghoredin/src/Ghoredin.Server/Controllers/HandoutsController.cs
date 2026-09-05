using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ghoredin.Application.Handouts;

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
    }
}
