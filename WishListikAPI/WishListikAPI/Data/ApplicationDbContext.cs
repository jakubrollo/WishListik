namespace WishListikAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using WishListik.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserMapping> UserMappings { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemInWishlist> ItemsInWishlists { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemInWishlist>()
                .HasKey(iw => new { iw.ItemId, iw.WishlistId });

            modelBuilder.Entity<Friendship>()
                .HasKey(f => new { f.UserId1, f.UserId2 });

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.UserId2)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserMapping>()
            .HasKey(um => um.GoogleUserId);

            modelBuilder.Entity<UserMapping>()
                .HasOne(um => um.User)
                .WithMany()
                .HasForeignKey(um => um.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
