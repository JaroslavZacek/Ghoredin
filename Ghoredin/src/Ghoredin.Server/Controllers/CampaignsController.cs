using Ghoredin.Application.Campaigns;
using Ghoredin.Server.Requests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Ghoredin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        #region Get

        [HttpGet]
        public async Task<IActionResult> GetMyCampaigns()
        {
            var campaigns = await _campaignService.GetMyCampaignsAsync();

            return Ok(campaigns);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);

            if (campaign is null)
                return NotFound();

            return Ok(campaign);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var campaigns = await _campaignService.GetAvailableCampaignsAsync();

            return Ok(campaigns);
        }

        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCampaignCommand command)
        {
            var campaign = await _campaignService.CreateAsync(command);

            return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign);
        }

        [HttpPost("{id:guid}/join")]
        public async Task<IActionResult> Join(Guid id, [FromBody] JoinCampaignRequest request)
        {
            try 
            {
                await _campaignService.JoinAsync(new JoinCampaignCommand(id, request.CharacterId));

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
