using System.ComponentModel.DataAnnotations;

namespace CommunityNoticeboard.ViewModels
{
    public class CommentCreateViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        [Display(Name = "Your Comment")]
        public string Content { get; set; } = string.Empty;
    }
}
