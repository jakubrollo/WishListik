using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WishListik.Services
{
    // Base class for services that require authentication
    public abstract class AuthenticatedServiceBase
    {
        protected readonly HttpClient _httpClient; // HTTP client to be used for making requests to the API

        // Constructor to initialize the service with an HttpClient
        protected AuthenticatedServiceBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Set the authorization header for HTTP requests
        protected async Task SetAuthorizationHeaderAsync()
        {
            // Retrieve the stored access token from secure storage
            var token = await SecureStorage.GetAsync("access_token");

            // If the token is not null or empty, set it in the HTTP client's authorization header
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
