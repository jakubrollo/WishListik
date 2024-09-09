using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;

namespace WishListik.Services
{
    // Service for managing item reservations
    public class ItemReservationsService : AuthenticatedServiceBase
    {
        // Constructor to initialize the service with an HttpClient
        public ItemReservationsService(HttpClient httpClient) : base(httpClient) { }

        // Retrieve reservations for a specific item in a wishlist
        public async Task<string> GetReservationsAsync(int itemId, int wishlistId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving reservations and send the GET request
            string apiUrl = $"api/ItemReservations/{itemId}/{wishlistId}";
            var response = await _httpClient.GetStringAsync(apiUrl);
            return response;
        }

        // Reserve an item with the specified details
        public async Task<HttpResponseMessage> ReserveItemAsync(ReserveItemRequest request)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for creating a reservation and prepare the request content
            string apiUrl = "api/ItemReservations";
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            // Send the POST request to create the reservation
            var response = await _httpClient.PostAsync(apiUrl, content);
            return response;
        }

        // Cancel a reservation for a specific item in a wishlist
        public async Task<HttpResponseMessage> CancelReservationAsync(int itemId, int wishlistId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for deleting the reservation and send the DELETE request
            string apiUrl = $"api/ItemReservations/{itemId}/{wishlistId}";
            var response = await _httpClient.DeleteAsync(apiUrl);
            return response;
        }
    }
}
