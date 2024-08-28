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
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UsersController(ApplicationDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var googleID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userID = await _userService.GetInternalUserIdAsync(googleID);

            if (userID != id)
            {
                return Forbid();
            }

            var userData = await _context.Users.Where(u => u.UserId == id)
                                               .FirstOrDefaultAsync();

            if (userData == null)
            {
                return NotFound();
            }

            return Ok(userData);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIDClaim == null)
            {
                return Unauthorized();
            }

            var userID = int.Parse(userIDClaim.Value);

            var userData = await _context.Users.Where(u => u.UserId == userID)
                                               .FirstOrDefaultAsync();

            if (userData == null)
            {
                return NotFound();
            }

            return Ok(userData);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
    }
}
