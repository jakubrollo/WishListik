using System.Net.Http.Headers;
using System.Text.Json;
using WishListik.Models;

namespace WishListik
{
    public partial class UserPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private IList<ItemInWishlist> _wishlistItems = new List<ItemInWishlist>();

        public UserPage()
        {
            InitializeComponent();
            _httpClient = App.ServiceProvider.GetRequiredService<HttpClient>();
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                var apiUserUrl = "https://2052-185-224-112-65.ngrok-free.app/api/users/me";
                var apiWishlistUrl = "https://2052-185-224-112-65.ngrok-free.app/api/wishlists/me";
                var token = await SecureStorage.GetAsync("access_token");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userResponse = await _httpClient.GetStringAsync(apiUserUrl);
                var user = JsonSerializer.Deserialize<User>(userResponse);

                if (user != null)
                {
                    UserNameLabel.Text = user.UserName;
                    EmailLabel.Text = user.Email;
                    RegistrationDateLabel.Text = user.RegistrationDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    await DisplayAlert("Error", "User data could not be loaded.", "OK");
                }

                var wishlistResponse = await _httpClient.GetStringAsync(apiWishlistUrl);
                var wishlist = JsonSerializer.Deserialize<Wishlist>(wishlistResponse);

                if (wishlist != null)
                {
                    _wishlistItems = wishlist.ItemsInWishlist.ToList();
                    WishlistItemsCollectionView.ItemsSource = _wishlistItems;
                }
                else
                {
                    await DisplayAlert("Error", "Wishlist data could not be loaded.", "OK");
                }
            }
            catch (Exception ex)
            {
                await HandleUnauthorizedAccess();
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task HandleUnauthorizedAccess()
        {
            Application.Current.MainPage = new LoginPage();
        }
    }
}
