namespace CommunityNoticeboard.ViewModels;

public class MemberViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
}
