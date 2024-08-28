namespace WishListik
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(FriendsPage), typeof(FriendsPage));
            Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
        }
    }
}
