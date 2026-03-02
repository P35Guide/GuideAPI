using GuideAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GuideAPI.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserPlace> UserPlaces { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<FavoritePlace> FavoritePlaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramUser>(e =>
            {
                e.HasIndex(x => x.TelegramId).IsUnique();
            });

            modelBuilder.Entity<UserSettings>(e =>
            {
                e.HasOne(x => x.TelegramUser)
                    .WithOne(x => x.Settings)
                    .HasForeignKey<UserSettings>(x => x.TelegramUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FavoritePlace>(e =>
            {
                e.HasOne(x => x.TelegramUser)
                    .WithMany(x => x.FavoritePlaces)
                    .HasForeignKey(x => x.TelegramUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => new { x.TelegramUserId, x.PlaceId }).IsUnique();
            });
        }
    }
}
