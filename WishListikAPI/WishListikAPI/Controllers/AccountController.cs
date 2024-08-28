using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using WishListikAPI.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "account/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();

            var googleID = response.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var givenName = response.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var userID = await _userService.FindOrCreateAsync(googleID, givenName, email);

            var token = GenerateJwtToken(userID);

            var redirectUri = $"wishlistik://auth?token={token}";
            Console.WriteLine(redirectUri);
            return Redirect(redirectUri);
        }

        private string GenerateJwtToken(int userId)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
    };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //tohle kdyztak vrat na DateTime
            var currentEpoch = EpochTime.GetIntDate(DateTime.UtcNow);
            var expiryEpoch = currentEpoch + (30 * 60);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                notBefore: DateTimeOffset.FromUnixTimeSeconds(currentEpoch).UtcDateTime, 
                expires: DateTimeOffset.FromUnixTimeSeconds(expiryEpoch).UtcDateTime,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}

/*
namespace WishListikAPI.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("~/signin")]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("~/signin-google-endpoint")]
        public async Task<IActionResult> GoogleResponse()
        {
            Console.WriteLine("fasfas");
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest();

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

            return Json(claims);
        }

        [HttpGet("~/signout")]
        public IActionResult SignOutUser()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
*/