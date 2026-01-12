using CommunityNoticeboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommunityNoticeboard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Post entity
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(p => p.Content)
                    .IsRequired()
                    .HasMaxLength(5000);

                entity.Property(p => p.Category)
                    .IsRequired();

                entity.Property(p => p.CreatedAt)
                    .IsRequired();

                // Relationship: Post -> User
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for better query performance
                entity.HasIndex(p => p.CreatedAt);
                entity.HasIndex(p => p.IsPinned);
                entity.HasIndex(p => p.Category);
            });

            // Configure Comment entity
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Content)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(c => c.CreatedAt)
                    .IsRequired();

                // Relationship: Comment -> Post
                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship: Comment -> User
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

                // Index for better query performance
                entity.HasIndex(c => c.PostId);
                entity.HasIndex(c => c.CreatedAt);
            });
        }
    }
}
