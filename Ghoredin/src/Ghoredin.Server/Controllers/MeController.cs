using Ghoredin.Application.Users;
using Ghoredin.Infrastructure.Identity;
using Ghoredin.Server.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghoredin.Server.Controllers
{
    [Route("api/me")]
    [ApiController]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public MeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        

        #region Get

        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            return Ok(ToDto(user));
        }

        #endregion

        #region Put

        [HttpPut("nickname")]
        public async Task<IActionResult> SetNickname([FromBody] SetNicknameRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            user.Nickname = string.IsNullOrWhiteSpace(request.Nickname) ? null : request.Nickname.Trim();
            await _userManager.UpdateAsync(user);

            return Ok(ToDto(user));

        }

        #endregion
        //---------------------------------------------------------------------------------------
        //------------------------- Privátní metody ---------------------------------------------
        //---------------------------------------------------------------------------------------

        private static MeDto ToDto(ApplicationUser user)
        {
            var displayName = !string.IsNullOrWhiteSpace(user.Nickname) ? user.Nickname : user.Email;

            return new MeDto(user.Id, user.Email!, user.Nickname, displayName);
        }
    }
}
