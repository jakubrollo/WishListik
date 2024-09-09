using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;

namespace WishListik.Services
{
    // Service for managing friendships
    public class FriendshipsService : AuthenticatedServiceBase
    {
        // Constructor to initialize the service with an HttpClient
        public FriendshipsService(HttpClient httpClient) : base(httpClient) { }

        // Retrieve the list of friends for the current user
        public async Task<List<User>> GetMyFriendsAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint and send the GET request
            string apiUrl = "api/Friendships/me";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize the response into a list of User objects
            var friends = JsonSerializer.Deserialize<List<User>>(response) ?? new();
            return friends;
        }

        // Retrieve the wishlist of a specific friend by their ID
        public async Task<Wishlist> GetFriendWishlistAsync(int friendId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving the friend's wishlist and send the GET request
            string apiUrl = $"api/Friendships/{friendId}";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize the response into a Wishlist object
            var wishlist = JsonSerializer.Deserialize<Wishlist>(response);

            // Throw an exception if the wishlist is null
            if (wishlist is null)
            {
                throw new InvalidOperationException($"Wishlist for friend with ID {friendId} could not be retrieved or was null.");
            }

            return wishlist;
        }

        // Delete a friendship with a specific friend by their ID
        public async Task<HttpResponseMessage> DeleteFriendshipAsync(int friendId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for deleting the friendship and send the DELETE request
            string apiUrl = $"api/Friendships/{friendId}";
            var response = await _httpClient.DeleteAsync(apiUrl);
            return response;
        }
    }
}
