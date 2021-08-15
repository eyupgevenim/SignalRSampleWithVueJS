using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using User.API.Models;

/// <summary>
/// https://github.com/DaniJG/so-signalr/blob/master/server/Controllers/AccountController.cs
/// </summary>
namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private readonly UserManager<User.API.Data.Entities.Identity.User> _userManager;
        private readonly SignInManager<User.API.Data.Entities.Identity.User> _signInManager;
        private readonly IOptions<JwtOptions> _jwtStrings;

        public AuthController(UserManager<Data.Entities.Identity.User> userManager,
            SignInManager<Data.Entities.Identity.User> signInManager,
            IOptions<JwtOptions> jwtStrings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtStrings = jwtStrings;
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = ServiceCollectionExtensions.JWTAuthScheme)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return StatusCode(200);
        }

        [HttpGet("context")]
        [Authorize(AuthenticationSchemes = ServiceCollectionExtensions.JWTAuthScheme)]
        public Task<IActionResult> Context()
        {
            var @return = new Return
            {
                Success = true,
                Code = 200,
                Data = new
                {
                    name = this.User?.Identity?.Name,
                    email = this.User?.FindFirstValue(ClaimTypes.Email),
                    role = this.User?.FindFirstValue(ClaimTypes.Role),
                }
            };

            return Task.FromResult<IActionResult>(new ObjectResult(@return));
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    return new ObjectResult(new Return
                    {
                        Success = true,
                        Code = 200,
                        Data = GetToken(user)
                    });
                }
            }

            return new ObjectResult(new Return
            {
                Success = false,
                Code = 400,
                Message = "Login failed"
            });
        }

        private TokenModel GetToken(Data.Entities.Identity.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtStrings.Value.IssuerSigningKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_jwtStrings.Value.Expires);

            var token = new JwtSecurityToken(
                issuer: _jwtStrings.Value.ValidIssuer,
                audience: _jwtStrings.Value.ValidAudience,
                claims: claims,
                expires: expires,
                signingCredentials: signingCredentials
            );

            return new TokenModel
            {
                AccessToken = _tokenHandler.WriteToken(token),
                Expires = expires,
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }

    }
}
