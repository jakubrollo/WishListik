using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using WishListik.Models;

namespace WishListik
{
    public partial class FriendsPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public FriendsPage()
        {
            InitializeComponent();
            _httpClient = App.ServiceProvider.GetRequiredService<HttpClient>();
            LoadFriends();
        }

        private async void LoadFriends()
        {
            try
            {
                var apiUrl = "https://2052-185-224-112-65.ngrok-free.app/api/friendships/me";
                var token = await SecureStorage.GetAsync("access_token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetStringAsync(apiUrl);
                var friends = JsonSerializer.Deserialize<List<User>>(response);

                if (friends != null)
                {
                    FriendsListView.ItemsSource = new ObservableCollection<User>(friends);
                }
                else
                {
                    await DisplayAlert("Error", "Friends list could not be loaded.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnFriendSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is User selectedFriend)
            {
                var apiUrl = $"https://2052-185-224-112-65.ngrok-free.app/api/friendships/{selectedFriend.UserId}";
                var token = await SecureStorage.GetAsync("access_token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                try
                {
                    var response = await _httpClient.GetStringAsync(apiUrl);
                    var wishlist = JsonSerializer.Deserialize<Wishlist>(response);

                    await Navigation.PushAsync(new FriendWishlistPage(wishlist));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }
    }
}
