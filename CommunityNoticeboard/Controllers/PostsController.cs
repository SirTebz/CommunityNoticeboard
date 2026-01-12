using CommunityNoticeboard.Data;
using CommunityNoticeboard.Models;
using CommunityNoticeboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityNoticeboard.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(PostCategory? category, string searchTerm)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .AsQueryable();

            // Filter by category
            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Content.Contains(searchTerm));
            }

            var allPosts = await query.ToListAsync();

            var viewModel = new PostIndexViewModel
            {
                PinnedPosts = allPosts.Where(p => p.IsPinned).OrderByDescending(p => p.CreatedAt).ToList(),
                RegularPosts = allPosts.Where(p => !p.IsPinned).OrderByDescending(p => p.CreatedAt).ToList(),
                FilterCategory = category,
                SearchTerm = searchTerm ?? string.Empty
            };

            return View(viewModel);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                NewComment = new CommentCreateViewModel { PostId = post.Id },
                CanEdit = currentUserId == post.UserId || isAdmin,
                CanDelete = currentUserId == post.UserId || isAdmin,
                CanPin = isAdmin
            };

            return View(viewModel);
        }

        // GET: Posts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View(new PostCreateViewModel());
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var post = new Post
                {
                    Title = model.Title,
                    Content = model.Content,
                    Category = model.Category,
                    UserId = userId!,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(post);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Post created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (post.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var model = new PostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Category = post.Category,
                UserId = post.UserId
            };

            return View(model);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PostEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (post.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Title = model.Title;
                    post.Content = model.Content;
                    post.Category = model.Category;
                    post.UpdatedAt = DateTime.UtcNow;

                    _context.Update(post);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Post updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = post.Id });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (post.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Post deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Posts/TogglePin/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TogglePin(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsPinned = !post.IsPinned;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = post.IsPinned ? "Post pinned successfully!" : "Post unpinned successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
