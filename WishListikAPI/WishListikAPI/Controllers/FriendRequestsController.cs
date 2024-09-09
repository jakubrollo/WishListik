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
    public class FriendRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing friend request data

        // Constructor to initialize the controller with the database context
        public FriendRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/FriendRequests
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest([FromBody] int friendId)
        {
            // Retrieve the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId == 0)
            {
                return Unauthorized(); // Return 401 if the user ID is invalid or missing
            }

            // Check if the user is trying to send a request to themselves
            if (userId == friendId)
            {
                return BadRequest("You cannot send a friend request to yourself.");
            }

            // Check if the users are already friends
            var areAlreadyFriends = await _context.Friendships
                .AnyAsync(f => (f.UserId1 == userId && f.UserId2 == friendId) ||
                               (f.UserId1 == friendId && f.UserId2 == userId));

            if (areAlreadyFriends)
            {
                return BadRequest("You are already friends with this user.");
            }

            // Check if both users exist in the database
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            var friendExists = await _context.Users.AnyAsync(u => u.UserId == friendId);

            if (!userExists)
            {
                return NotFound("Requester user does not exist.");
            }

            if (!friendExists)
            {
                return NotFound("Requested friend does not exist.");
            }

            // Check if there is already an existing friend request
            var existingRequest = await _context.FriendRequests
                .AnyAsync(fr => fr.RequesterId == userId && fr.RequesteeId == friendId);

            if (existingRequest)
            {
                return BadRequest("Friend request already exists.");
            }

            // Create and add a new friend request
            var friendRequest = new FriendRequest
            {
                RequesterId = userId,
                RequesteeId = friendId
            };

            _context.FriendRequests.Add(friendRequest);

            try
            {
                await _context.SaveChangesAsync(); // Save the new friend request to the database
            }
            catch (Exception)
            {
                // Log the exception details if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the friend request.");
            }

            return Ok(friendRequest); // Return the created friend request
        }

        // POST: api/FriendRequests/accept
        [HttpPost("accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] int requestId)
        {
            // Retrieve the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId == 0)
            {
                return Unauthorized(); // Return 401 if the user ID is invalid or missing
            }

            // Retrieve the friend request to be accepted
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId && fr.RequesteeId == userId);

            if (request == null)
            {
                return BadRequest("Invalid friend request.");
            }

            // Retrieve the users involved in the request
            var requester = await _context.Users.FindAsync(request.RequesterId);
            var requestee = await _context.Users.FindAsync(userId);

            if (requester == null || requestee == null)
            {
                return NotFound("One or both users not found.");
            }

            // Add a friendship record for both users
            var friendship1 = new Friendship { UserId1 = request.RequesterId, UserId2 = userId };
            _context.Friendships.Add(friendship1);

            // Remove the accepted friend request from the database
            _context.FriendRequests.Remove(request);

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception)
            {
                // Log the exception details if necessary
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving changes.");
            }

            return Ok(); // Return success response
        }

        // POST: api/FriendRequests/reject
        [HttpPost("reject")]
        [Authorize]
        public async Task<IActionResult> RejectFriendRequest([FromBody] int requestId)
        {
            // Retrieve the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId == 0)
            {
                return Unauthorized(); // Return 401 if the user ID is invalid or missing
            }

            // Retrieve and remove the friend request to be rejected
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId && fr.RequesteeId == userId);

            if (request == null)
            {
                return BadRequest("Invalid request.");
            }

            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync(); // Save changes to the database

            return Ok(); // Return success response
        }

        // GET: api/FriendRequests/pending
        [HttpGet("pending")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetUsersWhoSentMeRequests()
        {
            // Retrieve the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || userId == 0)
            {
                return Unauthorized(); // Return 401 if the user ID is invalid or missing
            }

            // Retrieve all pending friend requests where the user is the requestee
            var requests = await _context.FriendRequests
                .Where(fr => fr.RequesteeId == userId)
                .Include(fr => fr.Requester) // Include details of the requester
                .ToListAsync();

            return Ok(requests); // Return the list of pending friend requests
        }
    }
}
