using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WishListikAPI.Data;
using WishListik.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing user data
        // Constructor to initialize the controller with the database context
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Retrieve the current user's ID from the claims
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null)
            {
                return Unauthorized(); // Return 401 if the user ID claim is missing
            }

            var userID = int.Parse(userIDClaim.Value);

            // Fetch the user data from the database
            var userData = await _context.Users
                .Where(u => u.UserId == userID)
                .FirstOrDefaultAsync();

            if (userData == null)
            {
                return NotFound(); // Return 404 if the user is not found
            }

            return Ok(userData); // Return the user data as a 200 OK response
        }

        // GET: api/Users/me/id
        [HttpGet("me/id")]
        [Authorize]
        public IActionResult GetCurrentUserId()
        {
            // Retrieve the current user's ID from the claims
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null)
            {
                return Unauthorized(); // Return 401 if the user ID claim is missing
            }

            var userID = int.Parse(userIDClaim.Value);
            return Ok(userID); // Return the user ID as a 200 OK response
        }
    }
}
