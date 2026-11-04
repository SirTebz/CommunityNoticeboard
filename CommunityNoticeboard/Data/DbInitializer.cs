using CommunityNoticeboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityNoticeboard.Data;

public static class DbInitializer
{
    public static async Task Initialize(
        IServiceProvider serviceProvider,
        bool seedDevelopmentData,
        string? bootstrapAdminEmail,
        string? bootstrapAdminPassword)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (context.Database.IsSqlServer())
            await context.Database.MigrateAsync();
        else
            await context.Database.EnsureCreatedAsync();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (!seedDevelopmentData)
        {
            await CreateBootstrapAdminIfConfigured(userManager, bootstrapAdminEmail, bootstrapAdminPassword);
            return;
        }

        var admin = await CreateUserIfMissing(userManager, "admin@noticeboard.com", "Admin@123", "Admin");
        var member = await CreateUserIfMissing(userManager, "user@noticeboard.com", "User@123", "User");

        if (await context.Posts.AnyAsync()) return;

        context.Posts.AddRange(
            new Post { Title = "Welcome to the Community Noticeboard!", Content = "This is your central hub for community announcements, events, and discussions. Feel free to share updates and connect with your neighbours.", Category = PostCategory.Announcement, IsPinned = true, CreatedAt = DateTime.UtcNow.AddDays(-7), UserId = admin.Id },
            new Post { Title = "Community Cleanup Day - This Saturday", Content = "Join us this Saturday at 9 AM for our monthly community cleanup. We will focus on the park area; gloves and bags are provided.", Category = PostCategory.Event, CreatedAt = DateTime.UtcNow.AddDays(-2), UserId = admin.Id },
            new Post { Title = "New Recycling Guidelines", Content = "The city has updated recycling guidelines. Please rinse containers and separate plastics by number before collection day.", Category = PostCategory.News, IsPinned = true, CreatedAt = DateTime.UtcNow.AddDays(-5), UserId = member.Id });
        await context.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> CreateUserIfMissing(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException($"Unable to create the development user {email}.");
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
        return user;
    }

    private static async Task CreateBootstrapAdminIfConfigured(UserManager<ApplicationUser> userManager, string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;
        if (await userManager.FindByEmailAsync(email) is not null) return;

        var admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded) throw new InvalidOperationException("Unable to create the production bootstrap administrator.");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}
