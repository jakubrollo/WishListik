using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;

namespace WishListik.Services
{
    // Service for managing wishlist operations
    public class WishlistsService : AuthenticatedServiceBase
    {
        // Constructor to initialize the service with an HttpClient
        public WishlistsService(HttpClient httpClient) : base(httpClient) { }

        // Retrieve the current user's wishlist
        public async Task<Wishlist?> GetMyWishlistAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving the current user's wishlist and send the GET request
            string apiUrl = "api/Wishlists/me";
            var wishlistResponse = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize the response into a Wishlist object
            var wishlist = JsonSerializer.Deserialize<Wishlist>(wishlistResponse);

            return wishlist;
        }

        // Add an item to the current user's wishlist
        public async Task<HttpResponseMessage> AddItemToWishlistAsync(Item item)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for adding an item to the wishlist and prepare the request content
            string apiUrl = "api/wishlists/me/items";
            var content = new StringContent(JsonSerializer.Serialize(item.ItemId), System.Text.Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await _httpClient.PostAsync(apiUrl, content);

            return response;
        }

        // Remove an item from the current user's wishlist
        public async Task<HttpResponseMessage> DeleteItemFromWishlistAsync(ItemInWishlist itemInWishlist)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for deleting an item from the wishlist and send the DELETE request
            string apiUrl = $"api/Wishlists/me/items/{itemInWishlist.ItemId}";
            var response = await _httpClient.DeleteAsync(apiUrl);

            return response;
        }
    }
}
