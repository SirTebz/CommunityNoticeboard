using CommunityNoticeboard.Data;
using CommunityNoticeboard.Models;
using CommunityNoticeboard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityNoticeboard.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var comment = new Comment
                {
                    Content = model.Content,
                    PostId = model.PostId,
                    UserId = userId!,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Comment added successfully!";
                return RedirectToAction("Details", "Posts", new { id = model.PostId });
            }

            return RedirectToAction("Details", "Posts", new { id = model.PostId });
        }

        // POST: Comments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int postId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (comment.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment deleted successfully!";
            return RedirectToAction("Details", "Posts", new { id = postId });
        }
    }
}
