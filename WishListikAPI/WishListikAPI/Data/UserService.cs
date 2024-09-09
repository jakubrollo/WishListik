using WishListik.Models;
using Microsoft.EntityFrameworkCore;

namespace WishListikAPI.Data
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext; // Database context for accessing user data

        // Constructor to initialize the UserService with the database context
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Finds an existing user using google ID or creates a new one based on google ID
        public async Task<int> FindOrCreateAsync(string googleId, string givenName, string email)
        {
            // Check if a user mapping for the given Google ID already exists
            var userMapping = await _dbContext.UserMappings
                                              .FirstOrDefaultAsync(um => um.GoogleUserId == googleId);

            if (userMapping != null)
            {
                // Return the existing user's ID if found
                return userMapping.UserId;
            }

            // Create a new user if no mapping exists
            var newUser = new User
            {
                UserName = givenName,
                Email = email,
                RegistrationDate = DateTime.UtcNow
            };

            // Add the new user to the database and save changes
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Create a new user mapping for the new user
            var newUserMapping = new UserMapping
            {
                GoogleUserId = googleId,
                UserId = newUser.UserId
            };

            // Add the new user mapping to the database and save changes
            _dbContext.UserMappings.Add(newUserMapping);
            await _dbContext.SaveChangesAsync();

            // Create a new wishlist for the new user
            var newWishlist = new Wishlist
            {
                UserId = newUser.UserId,
                Name = $"{newUser.UserName}'s wishlist", // Set the wishlist name
                Description = $"Wishlist for {newUser.UserName}", // Set the wishlist description
                ItemsInWishlist = new List<ItemInWishlist>() // Initialize the wishlist with no items
            };

            // Add the new wishlist to the database and save changes
            _dbContext.Wishlists.Add(newWishlist);
            await _dbContext.SaveChangesAsync();

            // Return the ID of the newly created user
            return newUser.UserId;
        }

        // Retrieves the internal user ID associated with a given Google ID
        public async Task<int?> GetInternalUserIdAsync(string googleId)
        {
            // Find the user mapping for the given Google ID
            var userMapping = await _dbContext.UserMappings
                                              .FirstOrDefaultAsync(um => um.GoogleUserId == googleId);

            // Return the user's ID if found, otherwise null
            return userMapping?.UserId;
        }
    }
}
