using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace WishListik
{
    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public LoginPage()
        {
            InitializeComponent();
            _httpClient = App.ServiceProvider.GetRequiredService<HttpClient>();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var apiLoginUrl = new Uri("https://2052-185-224-112-65.ngrok-free.app/account/login");
                var callbackUrl = new Uri("wishlistik://auth");

                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(apiLoginUrl, callbackUrl);

                string token = GetTokenFromAuthResult(authResult);

                if (!string.IsNullOrEmpty(token))
                {
                    SecureStorage.SetAsync("access_token", token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await DisplayAlert("Error", "Token not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during login: {ex.Message}");
                await DisplayAlert("Error", "Login failed. Please try again.", "OK");
            }
        }

        private string GetTokenFromAuthResult(WebAuthenticatorResult authResult)
        {
            if (authResult.Properties.TryGetValue("token", out string token))
            {
                return token;
            }
            return null;
        }
    }
}
