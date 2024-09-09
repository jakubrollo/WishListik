using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Maui.Controls;
using WishListik.Models;
using WishListik.Services;

namespace WishListik
{
    // Page for displaying and managing a friend's wishlist
    public partial class FriendWishlistPage : ContentPage
    {
        private Wishlist _wishlist; // The wishlist to be displayed
        private UsersService _usersService;
        private ItemReservationsService _itemReservationsService;
        private FriendshipsService _friendshipsService;
        private bool _isProgrammaticChange; // Flag to indicate if changes are made programmatically
        private bool _isRefreshing; // Flag to indicate if data is being refreshed

        // Ctor for FriendWishlistPage
        public FriendWishlistPage(Wishlist wishlist)
        {
            InitializeComponent();
            BindingContext = this;
            _usersService = App.ServiceProvider!.GetRequiredService<UsersService>(); 
            _friendshipsService = App.ServiceProvider!.GetRequiredService<FriendshipsService>(); 
            _itemReservationsService = App.ServiceProvider!.GetRequiredService<ItemReservationsService>();
            _wishlist = wishlist; // Initialize wishlist
            WishlistNameLabel.Text = wishlist.Name; // Set the name of the wishlist
            ItemsListView.ItemsSource = _wishlist.ItemsInWishlist; // Set the source for the displayed items
            InitializeAsync(); // (update the wishlist and checkBoxes)
        }

        // Helper async initialization method
        private async void InitializeAsync()
        {
            await RefreshWishlist(); // Refresh the wishlist data
        }

        // Method to refresh wishlist data
        private async Task RefreshWishlist()
        {
            _wishlist = await _friendshipsService.GetFriendWishlistAsync(_wishlist.UserId); // Fetch updated wishlist
            ItemsListView.ItemsSource = _wishlist.ItemsInWishlist; // Update the items source for the list view of displayed items 
            await UpdateCheckBoxes(); // Update checkboxes based on item reservations
            UpdateItemReservationsDisplay(); // Update display of item reservations
        }

        // Method to update checkbox states based on item reservations
        private async Task UpdateCheckBoxes()
        {
            int? currentUserId = await _usersService.GetCurrentUserIdAsync(); // Retrieve current user ID
            if (currentUserId == null)
            {
                await DisplayAlert("Error", "User not authenticated.", "OK"); // Display error if user is not authenticated
                return;
            }

            _isProgrammaticChange = true; // Set flag to indicate programmatic changes

            foreach (var itemInWishlist in _wishlist.ItemsInWishlist)
            {
                var checkBox = FindCheckBoxForItem(itemInWishlist); // Find the checkbox for the item
                if (checkBox != null)
                {
                    checkBox.IsChecked = itemInWishlist.ItemReservations.Any(ir => ir.UserId == currentUserId.Value); // Set checkbox state
                }
            }

            _isProgrammaticChange = false; // Reset flag
        }

        // Method to find the checkbox for a given item
        private CheckBox? FindCheckBoxForItem(ItemInWishlist itemInWishlist)
        {
            foreach (var cell in ItemsListView.TemplatedItems.OfType<ViewCell>())
            {
                if (cell.BindingContext is ItemInWishlist item && item == itemInWishlist)
                {
                    return cell.FindByName<CheckBox>("ReservedCheckBox"); // Return the checkbox control
                }
            }
            return null; // Return null if no checkbox found
        }

        // Method to handle checkbox state change
        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is ItemInWishlist itemInWishlist)
            {
                if (_isProgrammaticChange)
                {
                    return; // Exit if changes are programmatic
                }

                var userId = await _usersService.GetCurrentUserIdAsync(); // Retrieve current user ID
                if (userId == null)
                {
                    await DisplayAlert("Error", "User not authenticated.", "OK"); // Display error if user is not authenticated
                    return;
                }

                HttpResponseMessage response;

                // Reserve or cancel reservation based on the checkBox state
                if (e.Value)
                {
                    response = await _itemReservationsService.ReserveItemAsync(new ReserveItemRequest
                    {
                        ItemId = itemInWishlist.ItemId,
                        WishlistId = itemInWishlist.WishlistId
                    }); // Reserve the item
                }
                else
                {
                    response = await _itemReservationsService.CancelReservationAsync(itemInWishlist.ItemId, itemInWishlist.WishlistId); // Cancel reservation
                }

                if (response.IsSuccessStatusCode)
                {
                    await RefreshWishlist(); // Refresh wishlist data
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update item reservation.", "OK"); // Display error if reservation update fails
                }
            }
        }

        // Method to update the display of item reservations
        private void UpdateItemReservationsDisplay()
        {
            foreach (var itemInWishlist in _wishlist.ItemsInWishlist)
            {
                var userNames = itemInWishlist.ItemReservations // Get user names who reserved the item
                    .Select(ir => ir.User?.UserName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();

                var displayText = userNames.Count() != 0 ? $"Reserved by: {string.Join(", ", userNames)}" : "No reservations"; // Create display text

                var viewCell = ItemsListView.TemplatedItems.OfType<ViewCell>() // Find the view cell for the item
                    .FirstOrDefault(vc => vc.BindingContext == itemInWishlist); 
                var label = viewCell?.FindByName<Label>("UserNamesLabel");     // Find the label for user names
                if (label != null)
                {
                    label.Text = displayText; // Update the reservation names label text
                }
            }
        }


        // Method called when the page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Navigation.PopAsync(); // Pop the page from the navigation stack
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

        // Command to refresh the wishlist data
        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true; // Set refreshing flag to true
            BindingContext = this; // Set the BindingContext to this page
            await RefreshWishlist(); // Refresh the wishlist data
            IsRefreshing = false; // Set refreshing flag to false
        });
    }
}
