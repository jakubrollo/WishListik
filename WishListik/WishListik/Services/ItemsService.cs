using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;

namespace WishListik.Services
{
    // Service for managing item-related operations
    public class ItemsService : AuthenticatedServiceBase
    {
        // Constructor to initialize the service with an HttpClient
        public ItemsService(HttpClient httpClient) : base(httpClient) { }

        // Retrieve the list of all items
        public async Task<List<Item>> GetItemsAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving items and send the GET request
            string apiUrl = "api/Items";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Check if the response is null and handle the error
            if (response is null)
            {
                throw new InvalidOperationException("The response from the server is null.");
            }

            // Deserialize the response into a list of Item objects
            var items = JsonSerializer.Deserialize<List<Item>>(response);

            // Check if the deserialized items list is null and handle the error
            if (items is null)
            {
                throw new InvalidOperationException("Deserialization resulted in a null value.");
            }

            return items;
        }

        // Retrieve a specific item by its ID
        public async Task<Item?> GetItemAsync(int id)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving a specific item and send the GET request
            string apiUrl = $"api/Items/{id}";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // Deserialize the response into an Item object
            var item = JsonSerializer.Deserialize<Item>(response);
            return item;
        }

        // Create a new item
        public async Task<HttpResponseMessage> PostItemAsync(Item item)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for creating an item and prepare the request content
            string apiUrl = "api/Items";
            var content = new StringContent(JsonSerializer.Serialize(item), System.Text.Encoding.UTF8, "application/json");

            // Send the POST request to create the item
            var response = await _httpClient.PostAsync(apiUrl, content);
            return response;
        }
    }
}
