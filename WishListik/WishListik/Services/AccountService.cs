using System.Net.Http.Headers;
using System.Net.Http.Json;
using WishListik.Models;

namespace WishListik.Services
{
    // Service class to handle account-related operations such as login and token management
    public class AccountService
    {
        private readonly HttpClient _httpClient; // HTTP client to make requests to the API

        // Constructor to initialize the service with an HttpClient
        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Retrieve the stored access token from secure storage
        public static async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("access_token");
        }

        // Store the access token in secure storage
        public static async Task StoreTokenAsync(string token)
        {
            await SecureStorage.SetAsync("access_token", token);
        }

        // Retrieve the stored refresh token from secure storage
        public static async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync("refresh_token");
        }

        // Store the refresh token in secure storage
        public static async Task StoreRefreshTokenAsync(string refreshToken)
        {
            await SecureStorage.SetAsync("refresh_token", refreshToken);
        }

        // Handle user login and store tokens in secure storage
        public async Task LoginAndStoreTokenAsync()
        {
            var apiLoginUrl = new Uri(_httpClient.BaseAddress + "account/login"); // API endpoint for login
            var callbackUrl = new Uri("wishlistik://auth"); // Callback URL for authentication (ANDROID)

            // Perform authentication using web authenticator
            WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(apiLoginUrl, callbackUrl);

            // Extract tokens from the authentication result
            string? token = GetTokenFromAuthResult(authResult);
            string? refreshToken = GetRefreshTokenFromAuthResult(authResult);

            // Store tokens if they are not null or empty
            if (!string.IsNullOrEmpty(token))
            {
                await StoreTokenAsync(token);
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await StoreRefreshTokenAsync(refreshToken);
                }
                else
                {
                    throw new Exception("RefreshToken not found.");
                }

                // Set the authorization header with the access token
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new Exception("Token not found.");
            }
        }

        // Refresh the access and refresh tokens
        public async Task RefreshTokensAsync()
        {
            var refreshToken = await GetRefreshTokenAsync();

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Refresh token not found.");
            }

            // Request new tokens using the refresh token
            var response = await _httpClient.PostAsJsonAsync("account/refresh", refreshToken);
            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokens != null)
                {
                    await StoreTokenAsync(tokens.AccessToken!);
                    await StoreRefreshTokenAsync(tokens.RefreshToken!);
                }
            }
            else
            {
                throw new Exception("Failed to refresh token.");
            }
        }

        // Extract the access token from the authentication result
        private static string? GetTokenFromAuthResult(WebAuthenticatorResult authResult)
        {
            if (authResult.Properties.TryGetValue("token", out string? token))
            {
                return token;
            }
            return null;
        }

        // Extract the refresh token from the authentication result
        private static string? GetRefreshTokenFromAuthResult(WebAuthenticatorResult authResult)
        {
            if (authResult.Properties.TryGetValue("refreshToken", out string? refreshToken))
            {
                return refreshToken;
            }
            return null;
        }
    }
}
