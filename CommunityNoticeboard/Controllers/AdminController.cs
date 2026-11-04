using CommunityNoticeboard.Data;
using CommunityNoticeboard.Models;
using CommunityNoticeboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunityNoticeboard.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = new DashboardViewModel
        {
            TotalPosts = await _context.Posts.CountAsync(),
            TotalMembers = await _userManager.Users.CountAsync(),
            TotalComments = await _context.Comments.CountAsync(),
            PinnedAnnouncements = await _context.Posts.CountAsync(p => p.IsPinned),
            RecentPosts = await _context.Posts.Include(p => p.User).Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt).Take(8).ToListAsync(),
            Categories = await _context.Posts.GroupBy(p => p.Category)
                .Select(g => new CategorySummaryViewModel { Name = g.Key.ToString(), Count = g.Count() })
                .OrderByDescending(x => x.Count).ToListAsync()
        };
        return View(dashboard);
    }

    public async Task<IActionResult> Members()
    {
        var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
        var model = new List<MemberViewModel>();
        foreach (var user in users)
        {
            model.Add(new MemberViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "Unknown",
                UserName = user.UserName ?? "Unknown",
                Roles = await _userManager.GetRolesAsync(user),
                PostCount = await _context.Posts.CountAsync(p => p.UserId == user.Id),
                CommentCount = await _context.Comments.CountAsync(c => c.UserId == user.Id)
            });
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(string userId, string role)
    {
        if (role is not ("Admin" or "User")) return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["ErrorMessage"] = "You cannot change your own administrator role.";
            return RedirectToAction(nameof(Members));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            TempData["ErrorMessage"] = "The member role could not be updated.";
            return RedirectToAction(nameof(Members));
        }

        var addResult = await _userManager.AddToRoleAsync(user, role);
        TempData[addResult.Succeeded ? "SuccessMessage" : "ErrorMessage"] = addResult.Succeeded
            ? $"{user.Email} is now a {role}."
            : "The member role could not be updated.";
        return RedirectToAction(nameof(Members));
    }
}
