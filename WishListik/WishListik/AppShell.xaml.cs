namespace WishListik
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("FriendsPage", typeof(FriendsPage));
            Routing.RegisterRoute("UserPage", typeof(UserPage));
        }
    }
}
