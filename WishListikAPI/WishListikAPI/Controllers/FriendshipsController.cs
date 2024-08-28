using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WishListikAPI.Data;
using WishListik.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendshipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Friendships/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetMyFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var friends = await _context.Friendships
                .Where(f => f.UserId1 == int.Parse(userId) || f.UserId2 == int.Parse(userId))
                .Select(f => f.UserId1 == int.Parse(userId) ? f.User2 : f.User1)
                .ToListAsync();

            return Ok(friends);
        }

        [HttpGet("{friendId}")]
        public async Task<ActionResult<Wishlist>> GetFriendWishlist(int friendId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var isFriend = await _context.Friendships
                .AnyAsync(f => (f.UserId1 == int.Parse(userId) && f.UserId2 == friendId) ||
                               (f.UserId2 == int.Parse(userId) && f.UserId1 == friendId));

            if (!isFriend)
            {
                return Forbid(); //403 Forbidden if not friends
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.ItemsInWishlist)
                .ThenInclude(iw => iw.Item)
                .FirstOrDefaultAsync(w => w.UserId == friendId);

            if (wishlist == null)
            {
                return NotFound();
            }

            return wishlist;
        }

    }
}