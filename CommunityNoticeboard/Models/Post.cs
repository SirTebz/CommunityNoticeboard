using System.ComponentModel.DataAnnotations;

namespace CommunityNoticeboard.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public PostCategory Category { get; set; }

        public bool IsPinned { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Key
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum PostCategory
    {
        Announcement = 1,
        Event = 2,
        News = 3,
        Discussion = 4,
        Help = 5,
        Other = 6
    }
}
