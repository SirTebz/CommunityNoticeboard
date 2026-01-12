using System.ComponentModel.DataAnnotations;

namespace CommunityNoticeboard.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int PostId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Post Post { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
