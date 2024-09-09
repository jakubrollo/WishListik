using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace WishListik.Services
{
    // Service for managing the current user-related operations
    public class UsersService : AuthenticatedServiceBase
    {
        // Cached user ID for performance optimization
        private int? _currentUserId;

        // Constructor to initialize the service with an HttpClient
        public UsersService(HttpClient httpClient) : base(httpClient) { }

        // Retrieve the current user details
        public async Task<User?> GetCurrentUserAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving the current user and send the GET request
            string apiUrl = "api/Users/me";
            var userResponse = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize the response into a User object and return it
            var user = JsonSerializer.Deserialize<User>(userResponse);
            return user;
        }

        // Retrieve the ID of the current user, either from cache or API
        public async Task<int?> GetCurrentUserIdAsync()
        {
            // Return the cached user ID if available
            if (_currentUserId.HasValue)
            {
                return _currentUserId;
            }

            // Fetch the user ID from the API if not cached
            var userId = await FetchUserIdFromApiAsync();
            if (userId.HasValue)
            {
                _currentUserId = userId.Value;
            }

            return _currentUserId;
        }

        // Fetch the current user's ID from the API
        private async Task<int?> FetchUserIdFromApiAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving the user ID and send the GET request
            var response = await _httpClient.GetAsync("api/users/me/id");

            // Check if the response is successful and parse the user ID
            if (response.IsSuccessStatusCode)
            {
                var userIdString = await response.Content.ReadAsStringAsync();
                return int.Parse(userIdString);
            }
            else
            {
                return null;
            }
        }

        // Reset the cached user ID (useful for logout or user change)
        public void Reset()
        {
            _currentUserId = null;
        }
    }
}
