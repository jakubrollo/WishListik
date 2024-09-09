namespace WishListik.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public Wishlist? Wishlist { get; set; }
        public UserMapping? UserMapping { get; set; }
        public ICollection<Friendship> FriendshipsAsUser1 { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsAsUser2 { get; set; } = new List<Friendship>();
        public ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
        public ICollection<ItemReservation> ItemReservations { get; set; } = new List<ItemReservation>();
    }

}
