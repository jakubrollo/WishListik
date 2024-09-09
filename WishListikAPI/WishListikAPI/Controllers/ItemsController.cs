using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WishListikAPI.Data;
using WishListik.Models;

namespace WishListikAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing item data

        // Constructor to initialize the controller with the database context
        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            // Retrieve and project all items to a list of ItemDto objects
            var items = await _context.Items
                .Select(item => new ItemDto
                {
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price
                })
                .ToListAsync();

            return Ok(items); // Return the list of items as a 200 OK response
        }

        // GET: api/Items/2
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            // Retrieve a single item by its ID
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound(); // Return 404 if the item is not found
            }

            return item; // Return the item as a 200 OK response
        }

        // POST: api/Items
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            // Add a new item to the database
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Return a 201 Created response with the location of the new item
            return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, item);
        }
    }
}
