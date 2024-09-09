namespace WishListik.Models
{
    public class UserMapping
    {
        public string? GoogleUserId { get; set; }
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}
