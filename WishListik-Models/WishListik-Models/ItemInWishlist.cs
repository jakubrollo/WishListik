namespace WishListik.Models
{
    public class ItemInWishlist
    {
        public int ItemId { get; set; }
        public int WishlistId { get; set; }
        public bool IsTaken { get; set; }

        // Navigation properties
        public Item Item { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
