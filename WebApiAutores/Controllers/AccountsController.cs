using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userMangaer;
        private readonly IConfiguration config;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtector dataProtector;

        public AccountsController(UserManager<IdentityUser> userMangaer, IConfiguration config, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider)
        {
            this.userMangaer = userMangaer;
            this.config = config;
            this.signInManager = signInManager;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico");
        }

        [HttpPost("register", Name ="RegistarUsuario")]
        public async Task<ActionResult<AutenticateResponse>> Register(UserCredentials userCredentials)
        {
            var usuario = new IdentityUser() 
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email
            };
            var resultado = await userMangaer.CreateAsync(usuario,userCredentials.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            return await BuildToken(userCredentials);
        }

        [HttpPost("login",Name ="LoginUsuario")]
        public async Task<ActionResult<AutenticateResponse>> Login(UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email,userCredentials.Password,isPersistent:false, lockoutOnFailure:false);

            if (!result.Succeeded)
            {
                return BadRequest("Login Incorrecto");
            }

            return await BuildToken(userCredentials);

        }

        [HttpGet("RenovarToken", Name ="RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AutenticateResponse>> Renovar()
        {
            var userEmailClaim = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            var email = userEmailClaim.Value;

            var userCredentials = new UserCredentials()
            {
                Email = email
            };

            return await BuildToken(userCredentials);
        }

        [HttpPost("DoAdmin",Name ="HacerAdmin")]
        public async Task<ActionResult> DoAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userMangaer.FindByEmailAsync(editAdminDTO.Email);
            if(user == null)
            {
                return NotFound();
            }

            await userMangaer.AddClaimAsync(user, new Claim("IsAdmin","true"));

            return NoContent();
        }

        [HttpPost("DoRemoveAdmin",Name ="RemoverAdmin")]
        public async Task<ActionResult> DoRemoveAdmin(EditAdminDTO editAdminDTO)
        {
            var user = await userMangaer.FindByEmailAsync(editAdminDTO.Email);

            if (user == null)
            {
                return NotFound();
            }

            await userMangaer.RemoveClaimAsync(user, new Claim("IsAdmin", "true"));

            return NoContent();
        }


        private async Task<AutenticateResponse> BuildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",userCredentials.Email)
            };

            var user = await userMangaer.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await userMangaer.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSecret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(1);

            var securityToken = new JwtSecurityToken(issuer:null,audience:null,claims:claims,expires:expiration,signingCredentials:creds);

            return new AutenticateResponse() 
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration,
            };
        }

    }
}
