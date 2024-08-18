namespace WishListikAPI.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public ICollection<ItemInWishlist> ItemsInWishlist { get; set; }
    }
}
