using WishListik.Models;

namespace WishListik
{
    public partial class FriendWishlistPage : ContentPage
    {
        public FriendWishlistPage(Wishlist wishlist)
        {
            InitializeComponent();

            WishlistNameLabel.Text = wishlist.Name;
            ItemsListView.ItemsSource = wishlist.ItemsInWishlist.Select(iw => iw.Item);
        }
    }
}
