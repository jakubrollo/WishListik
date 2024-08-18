namespace WishListik.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public User User { get; set; }
        public ICollection<ItemInWishlist> ItemsInWishlist { get; set; }
    }

}
