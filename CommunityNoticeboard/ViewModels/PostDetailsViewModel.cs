using CommunityNoticeboard.Models;

namespace CommunityNoticeboard.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; } = null!;
        public CommentCreateViewModel NewComment { get; set; } = new();
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPin { get; set; }
    }
}
