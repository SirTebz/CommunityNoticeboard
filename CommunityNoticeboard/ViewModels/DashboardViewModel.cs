using CommunityNoticeboard.Models;

namespace CommunityNoticeboard.ViewModels;

public class DashboardViewModel
{
    public int TotalPosts { get; set; }
    public int TotalMembers { get; set; }
    public int TotalComments { get; set; }
    public int PinnedAnnouncements { get; set; }
    public int MyPosts { get; set; }
    public int MyComments { get; set; }
    public List<Post> RecentPosts { get; set; } = new();
    public List<Post> PinnedPosts { get; set; } = new();
    public List<CategorySummaryViewModel> Categories { get; set; } = new();
}

public class CategorySummaryViewModel
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}
