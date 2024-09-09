namespace WishListik.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        // Navigation properties
        public ICollection<ItemInWishlist> ItemsInWishlist { get; set; } = new List<ItemInWishlist>();
    }

    // Maybe not necessary (TODO)

    public class ItemDto
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
