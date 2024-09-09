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
        private readonly ApplicationDbContext _context; // Database context for accessing friendship data

        // Constructor to initialize the controller with the database context
        public FriendshipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Friendships/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetMyFriends()
        {
            // Retrieve the current user ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID is missing
            }

            // Retrieve the list of friends for the current user
            var friends = await _context.Friendships
                .Where(f => f.UserId1 == int.Parse(userId) || f.UserId2 == int.Parse(userId))
                .Select(f => f.UserId1 == int.Parse(userId) ? f.User2 : f.User1)
                .ToListAsync();

            return Ok(friends); // Return the list of friends
        }

        // GET: api/Friendships/{friendId}
        [HttpGet("{friendId}")]
        [Authorize]
        public async Task<ActionResult<Wishlist>> GetFriendWishlist(int friendId)
        {
            // Retrieve the current user ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID is missing
            }

            // Check if the current user and the friend are in a friendship
            var isFriend = await _context.Friendships
                .AnyAsync(f => (f.UserId1 == int.Parse(userId) && f.UserId2 == friendId) ||
                               (f.UserId2 == int.Parse(userId) && f.UserId1 == friendId));

            if (!isFriend)
            {
                return Forbid(); // Return 403 if the users are not friends
            }

            // Retrieve the friend's wishlist along with associated items and reservations
            var wishlist = await _context.Wishlists
                .Include(w => w.ItemsInWishlist)
                    .ThenInclude(iw => iw.Item)
                .Include(w => w.ItemsInWishlist)
                    .ThenInclude(iw => iw.ItemReservations)
                        .ThenInclude(ir => ir.User)
                .FirstOrDefaultAsync(w => w.UserId == friendId);

            if (wishlist == null)
            {
                return NotFound(); // Return 404 if the wishlist is not found
            }

            return wishlist; // Return the wishlist
        }

        // DELETE: api/Friendships/{friendId}
        [HttpDelete("{friendId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFriendship(int friendId)
        {
            // Retrieve the current user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId == 0)
            {
                return Unauthorized(); // Return 401 if the user ID is invalid or missing
            }

            // Find and remove the friendship entry where the current user is UserId1
            var friendship1 = await _context.Friendships
                .FirstOrDefaultAsync(f => f.UserId1 == userId && f.UserId2 == friendId);

            if (friendship1 != null)
            {
                _context.Friendships.Remove(friendship1);
                await _context.SaveChangesAsync(); // Save changes to the database

                return NoContent(); // Return 204 indicating successful deletion
            }
            else
            {
                // Find and remove the friendship entry where the current user is UserId2
                var friendship2 = await _context.Friendships
                    .FirstOrDefaultAsync(f => f.UserId1 == friendId && f.UserId2 == userId);

                if (friendship2 != null)
                {
                    _context.Friendships.Remove(friendship2);
                    await _context.SaveChangesAsync(); // Save changes to the database

                    return NoContent(); // Return 204 indicating successful deletion
                }
                else
                {
                    return NotFound("Friendship not found"); // Return 404 if the friendship is not found
                }
            }
        }
    }
}
