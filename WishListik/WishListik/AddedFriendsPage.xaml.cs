using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text.Json;
using WishListik.Models;
using System.Text;
using System.Windows.Input;
using WishListik.Services;

namespace WishListik
{
    public partial class AddedFriendsPage : ContentPage
    {
        private FriendshipsService _friendshipsService;
        private bool _isRefreshing;
        private ObservableCollection<User> _friends = new();

        // Ctor for AddedFriendsPage
        public AddedFriendsPage()
        {
            BindingContext = this;
            InitializeComponent();
            _friendshipsService = App.ServiceProvider!.GetRequiredService<FriendshipsService>();
            InitializeAsync(); // Load friends
        }

        // Helper async initialization method
        private async void InitializeAsync()
        {
            await LoadFriends();
        }

        // Loads the list of friends and updates the UI
        private async Task LoadFriends()
        {
            try
            {
                // Fetch friends list from the service
                var friends = await _friendshipsService.GetMyFriendsAsync();
                _friends = new ObservableCollection<User>(friends);
                FriendsListView.ItemsSource = _friends;

                // Display a message if no friends are found (not necessary, but funny)
                if (friends.Count == 0)
                {
                    await DisplayAlert("Info", "You have no friends added.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle and display errors if any occur
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // Handles the event when a friend is selected from the list
        private async void OnFriendSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is User selectedFriend)
            {
                try
                {
                    // Fetch and navigate to the friend's wishlist page
                    var wishlist = await _friendshipsService.GetFriendWishlistAsync(selectedFriend.UserId);
                    await Navigation.PushAsync(new FriendWishlistPage(wishlist));
                }
                catch (Exception ex)
                {
                    // Handle and display errors if any occur
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        // Handles the event when the "Unfriend" button is clicked
        private async void OnUnfriendButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is User selectedFriend)
            {
                // Confirm unfriend action
                var result = await DisplayAlert("Unfriend", $"Are you sure you want to unfriend {selectedFriend.UserName}?", "Yes", "No");
                if (!result)
                    return;

                try
                {
                    // Remove the friend from the list
                    var response = await _friendshipsService.DeleteFriendshipAsync(selectedFriend.UserId);

                    if (response.IsSuccessStatusCode)
                    {
                        _friends.Remove(selectedFriend);
                        await DisplayAlert("Success", $"{selectedFriend.UserName} has been unfriended.", "OK");
                    }
                    else
                    {
                        // Handle and display error if unfriending failed
                        await DisplayAlert("Error", "Failed to unfriend.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    // Handle and display errors if any occur
                    await DisplayAlert("Error", $"An internal error occurred: {ex.Message}", "OK");
                }
            }
        }

        // Navigates to the friend requests page
        private async void OnFriendRequestsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FriendRequestsPage());
        }


        // Reset selected item when the page appears
        protected override void OnAppearing()
        {
            base.OnAppearing();
            FriendsListView.SelectedItem = null;
        }


        // Property to handle refreshing state
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        // Command for the refresh button to reload the friends list
        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            await LoadFriends();
            IsRefreshing = false;
        });
    }
}
