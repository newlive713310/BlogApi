using Adapter.BlogApi.Services.Helpers;
using Adapter.BlogApi.Services.Models;
using Adapter.BlogApi.Services.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Adapter.BlogApi.Services.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [EnableCors()]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationContext _context;
        public AuthController(ApplicationContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Registration new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Registration([FromBody] User request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (_context.Users.Find(request.Id) != null)
                return BadRequest("User is already exist!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.Created = DateTime.UtcNow;

            _context.Users.Add(request);

            await _context.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] User request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            if (_context.Users.FirstOrDefault(x => x.Email == request.Email) == null)
                return BadRequest("Invalid Email or password!");

            var identity = GetIdentity(request.Email, request.Password);

            if (identity == null)
                return BadRequest(new { errorText = "Invalid Email or password!" });

            var now = DateTime.UtcNow;
            // Generate JWT
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                email = identity.Name,
                userId = _context.Users.FirstOrDefault(x => x.Email == request.Email).Id
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            bool _validPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (user != null && _validPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
