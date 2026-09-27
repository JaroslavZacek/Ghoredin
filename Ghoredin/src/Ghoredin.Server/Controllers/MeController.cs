using Ghoredin.Application.Users;
using Ghoredin.Infrastructure.Identity;

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


        [HttpGet]
        public async Task<IActionResult> GetMe()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            return Ok(ToDto(user));
        }


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
