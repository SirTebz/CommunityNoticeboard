using CommunityNoticeboard.Data;
using CommunityNoticeboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommunityNoticeboard.Models;

namespace CommunityNoticeboard.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var model = new DashboardViewModel
        {
            TotalPosts = await _context.Posts.CountAsync(),
            TotalMembers = await _userManager.Users.CountAsync(),
            TotalComments = await _context.Comments.CountAsync(),
            PinnedAnnouncements = await _context.Posts.CountAsync(p => p.IsPinned),
            MyPosts = await _context.Posts.CountAsync(p => p.UserId == userId),
            MyComments = await _context.Comments.CountAsync(c => c.UserId == userId),
            RecentPosts = await _context.Posts.AsNoTracking().Include(p => p.User).Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync(),
            PinnedPosts = await _context.Posts.AsNoTracking().Include(p => p.User).Include(p => p.Comments)
                .Where(p => p.IsPinned).OrderByDescending(p => p.CreatedAt).Take(3).ToListAsync(),
            Categories = await _context.Posts.AsNoTracking().GroupBy(p => p.Category)
                .Select(g => new CategorySummaryViewModel { Name = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count).Take(5).ToListAsync()
        };
        return View(model);
    }
}
