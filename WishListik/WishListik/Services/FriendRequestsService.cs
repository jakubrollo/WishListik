using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using WishListik.Models;

namespace WishListik.Services
{
    // Service for managing friend requests
    public class FriendRequestsService : AuthenticatedServiceBase
    {
        // Constructor to initialize the service with an HttpClient
        public FriendRequestsService(HttpClient httpClient) : base(httpClient) { }

        // Send a friend request to a user with the specified ID
        public async Task<HttpResponseMessage> SendFriendRequestAsync(int friendId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for sending a friend request and prepare the request content
            string apiUrl = $"api/FriendRequests?friendId={friendId}";
            var requestContent = new StringContent(JsonSerializer.Serialize(friendId), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, requestContent); // Send the POST request
            return response;
        }

        // Accept a friend request with the specified request ID
        public async Task<HttpResponseMessage> AcceptFriendRequestAsync(int requestId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for accepting a friend request and prepare the request content
            string apiUrl = $"api/FriendRequests/accept?requestId={requestId}";
            var requestContent = new StringContent(JsonSerializer.Serialize(requestId), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, requestContent); // Send the POST request
            return response;
        }

        // Reject a friend request with the specified request ID
        public async Task<HttpResponseMessage> RejectFriendRequestAsync(int requestId)
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for rejecting a friend request and prepare the request content
            string apiUrl = $"api/FriendRequests/reject?requestId={requestId}"; 
            var requestContent = new StringContent(JsonSerializer.Serialize(requestId), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, requestContent); // Send the POST request
            return response;
        }

        // Retrieve a list of friend requests sent to the current user
        public async Task<List<FriendRequest>?> GetUsersWhoSentMeRequestsAsync()
        {
            await SetAuthorizationHeaderAsync();

            // Define the API endpoint for retrieving pending friend requests and send the GET request
            string apiUrl = "api/FriendRequests/pending";
            var response = await _httpClient.GetStringAsync(apiUrl); 
            var requests = JsonSerializer.Deserialize<List<FriendRequest>>(response); // Deserialize the response into a list of FriendRequest objects
            return requests; 
        }
    }
}
