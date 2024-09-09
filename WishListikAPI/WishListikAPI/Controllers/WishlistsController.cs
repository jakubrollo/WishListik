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
    public class WishlistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing wishlist data

        // Constructor to initialize the controller with the database context
        public WishlistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Wishlists/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<Wishlist>> GetMyWishlist()
        {
            // Retrieve the current user's ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID claim is missing
            }

            // Fetch the wishlist for the current user, including items in the wishlist
            var wishlist = await _context.Wishlists
                                         .Include(w => w.ItemsInWishlist)
                                         .ThenInclude(iw => iw.Item)
                                         .FirstOrDefaultAsync(w => w.UserId == int.Parse(userId));

            if (wishlist == null)
            {
                return NotFound(); // Return 404 if the wishlist is not found
            }

            return wishlist; // Return the wishlist data as a 200 OK response
        }

        // POST: api/Wishlists/me/items
        [HttpPost("me/items")]
        [Authorize]
        public async Task<IActionResult> AddItemToWishlist([FromBody] int itemId)
        {
            // Retrieve the current user's ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID claim is missing
            }

            // Fetch the user's wishlist
            var wishlist = await _context.Wishlists
                .Include(w => w.ItemsInWishlist)
                .FirstOrDefaultAsync(w => w.UserId == int.Parse(userId));

            if (wishlist == null)
            {
                return NotFound(); // Return 404 if the wishlist is not found
            }

            // Fetch the item to be added to the wishlist
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                return NotFound(); // Return 404 if the item is not found
            }

            // Check if the item is already in the wishlist
            if (wishlist.ItemsInWishlist.Any(iw => iw.ItemId == item.ItemId))
            {
                return BadRequest("Item already in wishlist."); // Return 400 if the item is already in the wishlist
            }

            // Add the item to the wishlist
            wishlist.ItemsInWishlist.Add(new ItemInWishlist
            {
                ItemId = item.ItemId,
                WishlistId = wishlist.WishlistId,
            });

            await _context.SaveChangesAsync(); // Save changes to the database

            return Ok(); // Return 200 OK if the item is successfully added
        }

        // DELETE: api/Wishlists/me/items/{itemId}
        [HttpDelete("me/items/{itemId}")]
        [Authorize]
        public async Task<IActionResult> DeleteItemFromWishlist(int itemId)
        {
            // Retrieve the current user's ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID claim is missing
            }

            // Fetch the user's wishlist
            var wishlist = await _context.Wishlists
                .Include(w => w.ItemsInWishlist)
                .FirstOrDefaultAsync(w => w.UserId == int.Parse(userId));

            if (wishlist == null)
            {
                return NotFound(); // Return 404 if the wishlist is not found
            }

            // Find the item in the wishlist to be removed
            var itemInWishlist = wishlist.ItemsInWishlist.FirstOrDefault(iw => iw.ItemId == itemId);
            if (itemInWishlist == null)
            {
                return NotFound(); // Return 404 if the item is not found in the wishlist
            }

            // Remove the item from the wishlist
            wishlist.ItemsInWishlist.Remove(itemInWishlist);
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the item is successfully removed
        }
    }
}
