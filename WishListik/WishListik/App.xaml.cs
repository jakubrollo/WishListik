using System.Diagnostics;
using System.Net.Http.Headers;
using WishListik.Services;

namespace WishListik
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();

            // Configure HttpClient for the API with base address and headers
            services.AddHttpClient("WishListikAPI", client =>
            {
                client.BaseAddress = new Uri("https://0c74-185-224-112-65.ngrok-free.app/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            // Register services that depend on HttpClient
            services.AddSingleton<AccountService>(sp =>
                new AccountService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<FriendRequestsService>(sp =>
                new FriendRequestsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<FriendshipsService>(sp =>
                new FriendshipsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<ItemReservationsService>(sp =>
                new ItemReservationsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<ItemsService>(sp =>
                new ItemsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<UsersService>(sp =>
                new UsersService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));
            services.AddSingleton<WishlistsService>(sp =>
                new WishlistsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("WishListikAPI")));

            // Build the service provider with the registered services
            ServiceProvider = services.BuildServiceProvider();

            // Set the initial main page of the application to LoadingPage
            MainPage = new NavigationPage(new LoadingPage());
        }

        // Method called when the application starts
        protected override async void OnStart()
        {
            base.OnStart();

            // Ensure that the ServiceProvider is initialized
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider not created.");
            }

            // Retrieve the AccountService from the service provider
            var accountService = ServiceProvider.GetRequiredService<AccountService>();

            try
            {
                // Attempt to get the refresh token
                var refreshToken = await AccountService.GetRefreshTokenAsync();

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // If the refresh token is available, attempt to refresh tokens and navigate to the main page
                    await accountService.RefreshTokensAsync();
                    MainPage = new NavigationPage(new AppShell());
                }
                else
                {
                    // If no refresh token is found, navigate to the login page
                    MainPage = new NavigationPage(new LoginPage());
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during token refresh and navigate to the login page
                Debug.WriteLine($"Error during token refresh: {ex.Message}");
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
