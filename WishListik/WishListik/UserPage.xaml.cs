using System.Windows.Input;
using WishListik.Models;
using WishListik.Services;

namespace WishListik
{
    // Represents the user page where user details and wishlisted items are displayed
    public partial class UserPage : ContentPage
    {
        private WishlistsService _wishlistsService;
        private UsersService _usersService;
        private List<ItemInWishlist> _wishlistItems = new(); // List of items in the user's wishlist

        public bool _isRefreshing { get; set; } // Flag indicating if the page is currently refreshing

        // Ctor for UserPage
        public UserPage()
        {
            BindingContext = this;
            InitializeComponent();
            _usersService = App.ServiceProvider!.GetRequiredService<UsersService>(); 
            _wishlistsService = App.ServiceProvider!.GetRequiredService<WishlistsService>();

            InitializeAsync(); // Load user data and wishlist items async
        }

        // Helper async initialization method
        private async void InitializeAsync()
        {
            await LoadUserDataAndWishlist(); // Load user data and wishlist items
        }

        // Loads user data and wishlist items, and handles errors
        private async Task LoadUserDataAndWishlist()
        {
            try
            {
                var user = await _usersService.GetCurrentUserAsync(); // Fetch current user data

                if (user != null)
                {
                    UserNameLabel.Text = user.UserName;
                    EmailLabel.Text = user.Email;
                    UserIdLabel.Text = $"ID: {user.UserId}";
                }
                else
                {
                    await DisplayAlert("Error", "User data could not be loaded.", "OK"); // Show error if user data fails to load
                }

                await RefreshWishlistAsync(); // Refresh the wishlist after loading user data
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); // Show error if an exception occurs
            }
        }

        // Refreshes the wishlist asynchronously and updates the UI
        private async Task RefreshWishlistAsync()
        {
            try
            {
                var wishlist = await _wishlistsService.GetMyWishlistAsync(); // Fetch the user's wishlist

                if (wishlist != null)
                {
                    _wishlistItems = wishlist.ItemsInWishlist?.ToList() ?? new(); // Update wishlist items
                    WishlistItemsCollectionView.ItemsSource = _wishlistItems; // Bind the items to the collection view

                    if (_wishlistItems.Count() == 0)
                    {
                        await DisplayAlert("Info", "Your wishlist is currently empty.", "OK"); // Notify if wishlist is empty
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Wishlist data could not be loaded.", "OK"); // Show error if wishlist data fails to load
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); // Show error if an exception occurs
            }
        }
        
        // Event handler for signing out the user
        private async void OnSignOutButtonClicked(object sender, EventArgs e)
        {
            await SecureStorage.SetAsync("access_token", string.Empty); // Clear access token
            await SecureStorage.SetAsync("refresh_token", string.Empty); // Clear refresh token
            _usersService.Reset(); // Reset user service state
            Application.Current!.MainPage = new LoginPage(); // Navigate to the login page
        }

        // Event handler for deleting an item from the user's wishlist
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var itemInWishlist = (ItemInWishlist)button.CommandParameter;

                var response = await _wishlistsService.DeleteItemFromWishlistAsync(itemInWishlist); // Delete item from wishlist

                if (response.IsSuccessStatusCode)
                {
                    await RefreshWishlistAsync(); // Refresh wishlist after deletion
                    await DisplayAlert("Success", "Item removed from wishlist.", "OK"); // Show success message
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Error", $"Unable to remove item: {errorMessage}", "OK"); // Show error message if deletion fails
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); // Show error if an exception occurs
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshWishlistAsync(); // Refresh wishlist when the page appears
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged("IsRefreshing"); // Notify that the property has changed
            }
        }

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true; // Start refreshing
            await RefreshWishlistAsync(); // Refresh the wishlist asynchronously
            IsRefreshing = false; // Stop refreshing
        });
    }
}
