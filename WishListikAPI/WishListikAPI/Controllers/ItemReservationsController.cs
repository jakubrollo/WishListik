using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WishListikAPI.Data;
using WishListik.Models;
using Microsoft.AspNetCore.Authorization;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing item reservation data

        // Constructor to initialize the controller with the database context
        public ItemReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ItemReservations/{itemId}/{wishlistId}
        [HttpGet("{itemId}/{wishlistId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetReservations(int itemId, int wishlistId)
        {
            // Retrieve reservations for a specific item and wishlist
            var reservations = await _context.ItemReservations
                .Where(ir => ir.ItemId == itemId && ir.WishlistId == wishlistId)
                .Include(ir => ir.User) // Include user details in the result
                .Select(ir => ir.User) // Select only the user details
                .ToListAsync();

            return Ok(reservations); // Return the list of users who have reserved the item
        }

        // POST: api/ItemReservations
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReserveItem([FromBody] ReserveItemRequest request)
        {
            // Retrieve the current user ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID is missing
            }

            // Check if the user has already reserved this item
            var existingReservation = await _context.ItemReservations
                .AnyAsync(ir => ir.ItemId == request.ItemId && ir.WishlistId == request.WishlistId && ir.UserId == int.Parse(userId));

            if (existingReservation)
            {
                return BadRequest("You have already reserved this item."); // Return 400 if reservation already exists
            }

            // Check if the item exists in the specified wishlist
            var itemInWishlist = await _context.ItemsInWishlists
                .AnyAsync(iw => iw.ItemId == request.ItemId && iw.WishlistId == request.WishlistId);

            if (itemInWishlist)
            {
                // Create a new reservation
                _context.ItemReservations.Add(new ItemReservation
                {
                    ItemId = request.ItemId,
                    WishlistId = request.WishlistId,
                    UserId = int.Parse(userId)
                });

                await _context.SaveChangesAsync(); // Save changes to the database
                return Ok(); // Return 200 indicating successful reservation
            }

            return NotFound("Item or wishlist not found."); // Return 404 if item or wishlist is not found
        }

        // DELETE: api/ItemReservations/{itemId}/{wishlistId}
        [HttpDelete("{itemId}/{wishlistId}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(int itemId, int wishlistId)
        {
            // Retrieve the current user ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Return 401 if the user ID is missing
            }

            // Find the reservation to cancel
            var reservation = await _context.ItemReservations
                .FirstOrDefaultAsync(ir => ir.ItemId == itemId && ir.WishlistId == wishlistId && ir.UserId == int.Parse(userId));

            if (reservation == null)
            {
                return NotFound("Reservation not found."); // Return 404 if the reservation does not exist
            }

            _context.ItemReservations.Remove(reservation); // Remove the reservation from the context
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 indicating successful cancellation
        }
    }
}
