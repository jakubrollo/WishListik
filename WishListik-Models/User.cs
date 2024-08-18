namespace WishListikAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Wishlist Wishlist { get; set; } // Navigation property
        public ICollection<Friendship> Friendships { get; set; }
    }

}
