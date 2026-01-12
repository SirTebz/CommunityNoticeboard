using Microsoft.AspNetCore.Identity;

namespace CommunityNoticeboard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
