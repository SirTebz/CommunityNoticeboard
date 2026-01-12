using CommunityNoticeboard.Models;
using System.ComponentModel.DataAnnotations;

namespace CommunityNoticeboard.ViewModels
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Post Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
        [Display(Name = "Post Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public PostCategory Category { get; set; }
    }
}
