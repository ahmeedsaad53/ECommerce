using E_Commerce_Api.Date;
using ECommerce.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        

        public AccController(UserManager<ApplicationUser>UserManager)
        {
            _UserManager = UserManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO UserFromRequest)
        {

            if (UserFromRequest == null)
                return BadRequest("Invalid request");

            if (ModelState.IsValid)
            {
                ApplicationUser user =new ApplicationUser();
                user.Email = UserFromRequest.Email;
                user.UserName = UserFromRequest.UserName;
                IdentityResult result = await _UserManager.CreateAsync(user, UserFromRequest.Password);
                if (result.Succeeded) {
                    return Ok("User Created");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest(ModelState);

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO UserFromRequest)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            ApplicationUser user = await _UserManager.FindByEmailAsync(UserFromRequest.Email);
            if (user == null) return BadRequest("Invalid UserName or Password");
            bool isPAsswordVild =await _UserManager.CheckPasswordAsync(user, UserFromRequest.Password);
            if (!isPAsswordVild) return BadRequest("Invalid UserName or Password");
            List<Claim> userClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName??""),
                new Claim(ClaimTypes.Email,user.Email??""),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sgdsgd648d9f*/w43U4354t69ts8e22365fh"));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userRoles = await _UserManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                userClaim.Add(new Claim(ClaimTypes.Role, role));
            }
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "http://localhost:5097",
                audience: "http://localhost:5097",
                claims: userClaim,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signingCredentials

                );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler()
                   .WriteToken(token),

                expiration = DateTime.Now.AddDays(7)
            });






            //http://localhost:5009/swagger/index.html






        }
    }
}
