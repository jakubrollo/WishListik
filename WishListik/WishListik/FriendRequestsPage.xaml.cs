using System.Collections.ObjectModel;
using System.Windows.Input;
using WishListik.Models;
using WishListik.Services;

namespace WishListik
{
    // Page for managing friend requests
    public partial class FriendRequestsPage : ContentPage
    {
        private FriendRequestsService _friendRequestsService; // Service for handling friend requests
        private bool _isRefreshing; // Flag to indicate if data is being refreshed

        // Ctor for FriendRequestsPAge
        public FriendRequestsPage()
        {
            InitializeComponent(); // Initialize UI components
            BindingContext = this;
            _friendRequestsService = App.ServiceProvider!.GetRequiredService<FriendRequestsService>();
        }

        // Method to handle page appearing
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFriendRequests();
        }

        // Method to load friend requests from the service
        private async Task LoadFriendRequests()
        {
            try
            {
                var requests = await _friendRequestsService.GetUsersWhoSentMeRequestsAsync(); // Fetch friend requests

                if (requests != null)
                {
                    // Set the ItemsSource of the ListView to the fetched friend requests
                    FriendRequestsListView.ItemsSource = new ObservableCollection<FriendRequest>(requests);
                }
                else
                {
                    await DisplayAlert("Error", "Friend requests could not be loaded.", "OK"); // Display error if no requests are found
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An internal error occurred: {ex.Message}.", "OK"); // Display error if an exception occurs
            }
        }

        // Method to handle clicking the send friend request button
        private async void OnSendFriendRequestClicked(object sender, EventArgs e)
        {
            var friendIdText = FriendIdSearchBar.Text; // Get the text from the search bar

            // Validate the input and parse friend ID
            if (string.IsNullOrWhiteSpace(friendIdText) || !int.TryParse(friendIdText, out int friendId))
            {
                await DisplayAlert("Error", "Please enter a valid friend ID.", "OK"); // Display error if input is invalid
                return;
            }

            try
            {
                var response = await _friendRequestsService.SendFriendRequestAsync(friendId); // Send friend request

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Friend request sent.", "OK"); // Display success message if request is sent
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", errorMessage, "OK"); // Display error message if request fails
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); // Display error if an exception occurs
            }
        }

        // Method to handle clicking the accept friend request button
        private async void OnAcceptFriendRequestClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.BindingContext is FriendRequest selectedFriendRequest)
                {
                    var response = await _friendRequestsService.AcceptFriendRequestAsync(selectedFriendRequest.FriendRequestId); // Accept the friend request

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", $"{selectedFriendRequest.Requester!.UserName} has been added as a friend.", "OK"); // Display success message if request is accepted
                        await LoadFriendRequests(); // Reload friend requests
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to accept the friend request.", "OK"); // Display error message if accepting request fails
                    }
                }
            }
        }

        // Method to handle clicking the decline friend request button
        private async void OnDeclineFriendRequestClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.BindingContext is User selectedUser)
                {
                    var response = await _friendRequestsService.RejectFriendRequestAsync(selectedUser.UserId); // Decline the friend request

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", $"Friend request from {selectedUser.UserName} has been declined.", "OK"); // Display success message if request is declined
                        await LoadFriendRequests(); // Reload friend requests
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to decline the friend request.", "OK"); // Display error message if rejecting request fails
                    }
                }
            }
        }

        // Property to handle refreshing state
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing)); // Notify that the property has changed
            }
        }

        // Command to refresh friend requests
        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            await LoadFriendRequests();
            IsRefreshing = false;
        });
    }
}
