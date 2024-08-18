using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WishListikAPI.Data;
using WishListik.Models;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WishlistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Wishlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlists()
        {
            return await _context.Wishlists.Include(w => w.ItemsInWishlist).ToListAsync();
        }

        // GET: api/Wishlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wishlist>> GetWishlist(int id)
        {
            var wishlist = await _context.Wishlists.Include(w => w.ItemsInWishlist)
                                                    .ThenInclude(iw => iw.Item)
                                                    .FirstOrDefaultAsync(w => w.WishlistId == id);

            if (wishlist == null)
            {
                return NotFound();
            }

            return wishlist;
        }

        // POST: api/Wishlists
        [HttpPost]
        public async Task<ActionResult<Wishlist>> PostWishlist(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.WishlistId }, wishlist);
        }
    }

}
