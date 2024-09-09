namespace WishListikAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using WishListik.Models;

    // DbContext class for configuring the application's database context
    public class ApplicationDbContext : DbContext
    {
        // Constructor to initialize the context with DbContextOptions
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // DbSets representing the tables in the database
        public DbSet<User> Users { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemInWishlist> ItemsInWishlists { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserMapping> UserMappings { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<ItemReservation> ItemReservations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Configures the model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for the ItemInWishlist entity
            modelBuilder.Entity<ItemInWishlist>()
                .HasKey(iw => new { iw.ItemId, iw.WishlistId });

            // Composite key for the ItemReservation entity
            modelBuilder.Entity<ItemReservation>()
                .HasKey(ir => new { ir.ItemId, ir.WishlistId, ir.UserId });

            // Relationships for ItemReservation
            modelBuilder.Entity<ItemReservation>()
                .HasOne(ir => ir.ItemInWishlist)
                .WithMany(iw => iw.ItemReservations)
                .HasForeignKey(ir => new { ir.ItemId, ir.WishlistId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemReservation>()
                .HasOne(ir => ir.User)
                .WithMany(u => u.ItemReservations)
                .HasForeignKey(ir => ir.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Primary key for the User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            // One-to-one relationship between User and Wishlist
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wishlist)
                .WithOne(w => w.User)
                .HasForeignKey<Wishlist>(w => w.UserId);

            // Primary key for the Wishlist entity
            modelBuilder.Entity<Wishlist>()
                .HasKey(w => w.WishlistId);

            // One-to-many relationship between Wishlist and ItemInWishlist
            modelBuilder.Entity<Wishlist>()
                .HasMany(w => w.ItemsInWishlist)
                .WithOne(iw => iw.Wishlist)
                .HasForeignKey(iw => iw.WishlistId);

            // Primary key for the Item entity
            modelBuilder.Entity<Item>()
                .HasKey(i => i.ItemId);

            // One-to-many relationship between Item and ItemInWishlist
            modelBuilder.Entity<Item>()
                .HasMany(i => i.ItemsInWishlist)
                .WithOne(iw => iw.Item)
                .HasForeignKey(iw => iw.ItemId);

            // Configures the column type for Price in Item entity
            modelBuilder.Entity<Item>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            // Composite key for the Friendship entity
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId1, f.UserId2 });

            // Relationships for Friendship
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany(u => u.FriendshipsAsUser1)
                .HasForeignKey(f => f.UserId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany(u => u.FriendshipsAsUser2)
                .HasForeignKey(f => f.UserId2)
                .OnDelete(DeleteBehavior.Restrict);

            // Add a check constraint to ensure UserId1 and UserId2 are not the same
            modelBuilder.Entity<Friendship>()
                .ToTable(t => t.HasCheckConstraint("CHK_Friendship_UserId1_UserId2", "[UserId1] <> [UserId2]"));

            // Primary key for the UserMapping entity
            modelBuilder.Entity<UserMapping>()
                .HasKey(um => um.GoogleUserId);

            // One-to-one relationship between UserMapping and User
            modelBuilder.Entity<UserMapping>()
                .HasOne(um => um.User)
                .WithOne(u => u.UserMapping)
                .HasForeignKey<UserMapping>(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Primary key for the FriendRequest entity
            modelBuilder.Entity<FriendRequest>()
                .HasKey(fr => fr.FriendRequestId);

            // Relationships for FriendRequest
            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Requester)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fr => fr.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Requestee)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.RequesteeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Primary key for the RefreshToken entity
            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id); // Set primary key

            // Relationship for RefreshToken
            modelBuilder.Entity<RefreshToken>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
