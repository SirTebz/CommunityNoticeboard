using CommunityNoticeboard.Models;

namespace CommunityNoticeboard.ViewModels
{
    public class PostIndexViewModel
    {
        public List<Post> PinnedPosts { get; set; } = new();
        public List<Post> RegularPosts { get; set; } = new();
        public PostCategory? FilterCategory { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }
}
