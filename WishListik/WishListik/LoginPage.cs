using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using WishListik.Services;

namespace WishListik
{
    // Represents the initial login page of the application
    public partial class LoginPage : ContentPage
    {
        private readonly AccountService _accountService; // Service for handling account-related operations

        // Ctor for LoginPage
        public LoginPage()
        {
            InitializeComponent();
            _accountService = App.ServiceProvider!.GetRequiredService<AccountService>(); // Retrieve the AccountService from the service provider
        }

        // Event handler for the login button click event
        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Attempt to log in and store the authentication token
                await _accountService.LoginAndStoreTokenAsync();

                // Navigate to the main application page upon successful login
                Application.Current!.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during login
                Debug.WriteLine($"Error during login: {ex.Message}"); // Log the error message
                await DisplayAlert("Error", "Login failed. Please try again.", "OK"); // Show an alert to the user
            }
        }
    }
}
