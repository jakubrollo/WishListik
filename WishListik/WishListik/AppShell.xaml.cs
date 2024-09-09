namespace WishListik
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            // Allows navigation to AddedFriendsPage using its route name
            Routing.RegisterRoute(nameof(AddedFriendsPage), typeof(AddedFriendsPage));

            // Allows navigation to UserPage using its route name
            Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
        }
    }
}
