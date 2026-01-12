using CommunityNoticeboard.Models;
using Microsoft.AspNetCore.Identity;

namespace CommunityNoticeboard.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@noticeboard.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Test User
            var testEmail = "user@noticeboard.com";
            var testUser = await userManager.FindByEmailAsync(testEmail);

            if (testUser == null)
            {
                testUser = new ApplicationUser
                {
                    UserName = testEmail,
                    Email = testEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, "User@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, "User");
                }
            }

            // Seed Sample Posts
            if (!context.Posts.Any())
            {
                var samplePosts = new List<Post>
                {
                    new Post
                    {
                        Title = "Welcome to the Community Noticeboard!",
                        Content = "This is your central hub for community announcements, events, and discussions. Feel free to share updates and connect with your neighbors.",
                        Category = PostCategory.Announcement,
                        IsPinned = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        UserId = adminUser.Id
                    },
                    new Post
                    {
                        Title = "Community Cleanup Day - This Saturday",
                        Content = "Join us this Saturday at 9 AM for our monthly community cleanup. We'll be focusing on the park area. Gloves and bags will be provided!",
                        Category = PostCategory.Event,
                        IsPinned = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UserId = adminUser.Id
                    },
                    new Post
                    {
                        Title = "New Recycling Guidelines",
                        Content = "The city has updated recycling guidelines. Please make sure to rinse all containers and separate plastics by number. More details on the city website.",
                        Category = PostCategory.News,
                        IsPinned = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UserId = testUser.Id
                    }
                };

                context.Posts.AddRange(samplePosts);
                await context.SaveChangesAsync();
            }
        }
    }
}
