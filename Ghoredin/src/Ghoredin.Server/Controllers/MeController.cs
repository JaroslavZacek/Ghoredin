using Ghoredin.Application.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghoredin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        private readonly ICurrentUserService _currentUser;
        
        public MeController(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        /// Vrací základní informace o právě přihlášeném uživateli.
        /// </summary>
        /// <returns>HTTP 200 (OK) s JSON objektem ve tvaru: { userId, isAuthenticated }</returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetMe() 
        {
            return Ok(new
            {
                userId = _currentUser.UserId,
                isAuthenticated = _currentUser.IsAuthenticated
            });
        }
    }
}
