namespace WishListik.Models
{
    public class ItemInWishlist
    {
        public int ItemId { get; set; }
        public int WishlistId { get; set; }

        // Navigation properties
        public Item? Item { get; set; }
        public Wishlist? Wishlist { get; set; }

        public ICollection<ItemReservation> ItemReservations { get; set; } = new List<ItemReservation>();
    }
}
