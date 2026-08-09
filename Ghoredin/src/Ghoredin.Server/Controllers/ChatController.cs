using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Ghoredin.Application.Chat;
using Ghoredin.Server.Requests;

namespace Ghoredin.Server.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        #region Get

        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetHistory(Guid campaignId)
        {
            try
            {
                var messages = await _chatService.GetVisibleHistoryAsync(campaignId);

                return Ok(messages);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Post

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            try
            {
                var command = new SendMessageCommand(request.CampaignId, request.Content, request.WhisperToUserId);
                var message = await _chatService.SendMessageAsync(command);

                return Ok(message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        #endregion
    }
}
