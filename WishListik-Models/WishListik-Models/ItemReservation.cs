namespace WishListik.Models
{
    public class ItemReservation
    {
        public int ItemId { get; set; }
        public int WishlistId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public ItemInWishlist? ItemInWishlist { get; set; }
        public User? User { get; set; }
    }

    public class ReserveItemRequest
    {
        public int ItemId { get; set; }
        public int WishlistId { get; set; }
    }
}
