using EditorAPI.Data.Entities;
using EditorAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EditorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("/register", Name = "Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (_userManager.FindByNameAsync(registerModel.Username).Result == null)
            {
                var user = new ApplicationUser();
                user.UserName = registerModel.Username;
                user.Email = registerModel.Email;

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    return Ok(
                        new
                        {
                            userId = user.Id
                        });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(IdentityResult.Failed(
                new IdentityError
                {
                    Description = "User Already Exist"
                }));
        }
        [HttpPost("/auth")]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username!);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {

                var authClaims = await FormClaimsAsync(user);
                var token = GetToken(authClaims);

                return Ok(new
                {
                    userId = user.Id,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
        private async Task<List<Claim>> FormClaimsAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            return authClaims;
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var secret = _configuration["JWT:Secret"] ?? throw new Exception("Jwr secter is not set");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
