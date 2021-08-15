using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IEnumerable<Claim> GetCurrentUserClaims => this.User.Identity.IsAuthenticated ? this.User.Claims : new List<Claim>();
        protected string GetCurrentUserId => GetCurrentUserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
        protected string GetCurrentUserName => GetCurrentUserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    }
}
