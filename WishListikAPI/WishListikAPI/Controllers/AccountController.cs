using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WishListikAPI.Data;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserService _userService; // Service to manage user-related operations
        private readonly TokenService _tokenService; // Service to handle token generation and validation

        private readonly IConfiguration _configuration; // Configuration settings for the application
        private readonly ApplicationDbContext _context; // Database context for accessing data

        // Constructor to initialize services and context
        public AccountController(UserService userService, TokenService tokenService, IConfiguration configuration, ApplicationDbContext context)
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
            _context = context;
        }

        // to initiate Google authentication
        //
        // GET: account/login
        [HttpGet("login")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "account/signin-google" };
            // Redirect to Google for authentication
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        // Endpoint to handle the Google login callback (has to stay like this -> Google policy)
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            // Authenticate
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (response.Principal == null)
                return BadRequest(); // Return BadRequest if authentication fails

            // Extract user information from the authenticated principal
            var googleID = response.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var givenName = response.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = response.Principal.FindFirstValue(ClaimTypes.Email);

            // Validate extracted information
            if (string.IsNullOrEmpty(googleID) || string.IsNullOrEmpty(givenName) || string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("Required Google account information is missing.");
            }

            // Find or create a user in the database
            var userID = await _userService.FindOrCreateAsync(googleID, givenName, email);

            // Generate access and refresh tokens
            var token = _tokenService.GenerateAccessToken(userID);
            var refreshToken = _tokenService.GenerateAndSaveRefreshToken(userID);

            // Construct redirect URI with tokens
            var redirectUri = $"wishlistik://auth?token={token}&refreshToken={refreshToken}";
            Console.WriteLine(redirectUri);
            return Redirect(redirectUri); // Redirect to the application with tokens
        }

        // to refresh access token using a refresh token
        //
        // GET: account/refresh
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] string refreshToken)
        {
            // Encrypt the provided refresh token
            var encryptedRefreshToken = refreshToken; // _tokenService.Encrypt(refreshToken);

            // Look up the encrypted token in the database
            var storedToken = _context.RefreshTokens
                .FirstOrDefault(rt => rt.Token == encryptedRefreshToken);

            if (storedToken == null)
                return Unauthorized("Invalid refresh token."); // Return Unauthorized if token is not found

            // Check if the token is expired
            if (DateTime.UtcNow > storedToken.Expiration)
                return Unauthorized("Expired refresh token."); // Return Unauthorized if token is expired

            // Generate a new access token
            var newAccessToken = _tokenService.GenerateAccessToken(storedToken.UserId);

            // Generate and save a new refresh token
            var newRefreshToken = _tokenService.GenerateAndSaveRefreshToken(storedToken.UserId);

            // Return new tokens to the client
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
