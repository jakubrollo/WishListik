using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WishListik.Models;
using WishListik.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace WishListik
{
    // Represents the main page of the application where the list of items to put into wishlist are shown
    public partial class MainPage : ContentPage
    {
        private ItemsService _itemsService;
        private WishlistsService _wishlistsService;
        private bool _isRefreshing; // Flag indicating if the page is currently refreshing

        public ObservableCollection<Item> Items { get; set; } = new(); // Collection of items displayed on the page

        // Ctor for MainPage
        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
            _itemsService = App.ServiceProvider!.GetRequiredService<ItemsService>(); 
            _wishlistsService = App.ServiceProvider!.GetRequiredService<WishlistsService>();
        }

        // Called when the page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadItems(); // Load items when the page appears
        }

        // Loads items from the ItemsService and updates the Items collection
        private async Task LoadItems()
        {
            try
            {
                Items.Clear(); // Clear existing items
                var items = await _itemsService.GetItemsAsync(); // Fetch items from the service
                foreach (var item in items)
                {
                    Items.Add(item); // Add each item to the collection
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to load items: {ex.Message}", "OK"); // Display error if loading fails
            }
        }

        // Event handler for adding an item to the wishlist
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.CommandParameter as Item;

            if (item != null)
            {
                try
                {
                    var response = await _wishlistsService.AddItemToWishlistAsync(item); // Add item to the wishlist

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Item added to wishlist.", "OK"); // Show success message
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Error", $"Unable to add item: {errorMessage}", "OK"); // Show error message
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK"); // Display error if an exception occurs
                }
            }
        }

        // Event handler for adding a new item using prompts. (Should be improved)
        private async void OnAddItemButtonClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("New Item", "Enter item name:");               // Prompt for item name
            string description = await DisplayPromptAsync("New Item", "Enter item description:"); // Prompt for item description
            string priceString = await DisplayPromptAsync("New Item", "Enter item price:");       // Prompt for item price

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(description) && decimal.TryParse(priceString, out decimal price))
            {
                var newItem = new Item
                {
                    Name = name,
                    Description = description,
                    Price = price
                };

                Items.Add(newItem); // Add the new item to the collection

                await _itemsService.PostItemAsync(newItem); // Send the new item to the service
                await LoadItems();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing)); // Notify that the property has changed
            }
        }

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true; // Start refreshing
            await LoadItems(); // Load items from the service
            IsRefreshing = false; // Stop refreshing
        });
    }
}
