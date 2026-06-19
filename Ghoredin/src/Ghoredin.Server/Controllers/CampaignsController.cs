using Ghoredin.Application.Campaigns;

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
        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCampaignCommand command)
        {
            var campaign = await _campaignService.CreateAsync(command);

            return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign);
        }

        #endregion
    }
}
