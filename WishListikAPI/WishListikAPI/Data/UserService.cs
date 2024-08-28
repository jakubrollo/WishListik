using WishListik.Models;
using Microsoft.EntityFrameworkCore; 

namespace WishListikAPI.Data
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> FindOrCreateAsync(string googleId, string givenName, string email)
        {
            var userMapping = await _dbContext.UserMappings
                                              .FirstOrDefaultAsync(um => um.GoogleUserId == googleId);

            if (userMapping != null)
            {
                return userMapping.UserId;
            }

            var newUser = new User
            {
                UserName = givenName,
                Email = email,
                RegistrationDate = DateTime.UtcNow
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            var newUserMapping = new UserMapping
            {
                GoogleUserId = googleId,
                UserId = newUser.UserId
            };

            _dbContext.UserMappings.Add(newUserMapping);
            await _dbContext.SaveChangesAsync();

            return newUser.UserId;
        }

        public async Task<int?> GetInternalUserIdAsync(string googleId)
        {
            var userMapping = await _dbContext.UserMappings
                                              .FirstOrDefaultAsync(um => um.GoogleUserId == googleId);

            return userMapping?.UserId;
        }
    }

}
