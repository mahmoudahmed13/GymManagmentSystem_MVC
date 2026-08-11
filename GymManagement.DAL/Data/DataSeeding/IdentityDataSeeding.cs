using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagement.DAL.Data.DataSeeding
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger logger,
            CancellationToken ct = default)
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync(ct);
                var hasRoles = await roleManager.Roles.AnyAsync(ct);

                var roles = new List<IdentityRole>()
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
            };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed to create role {role.Name} : {string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }
                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mahmoud",
                        LastName = "Ahmed",
                        Email = "mahmoud@gmail.com",
                        UserName = "MahmoudAhmed",
                        PhoneNumber = "01123435446"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ss0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Ali",
                        LastName = "Ahmed",
                        Email = "Ali@gmail.com",
                        UserName = "AliAhmed",
                        PhoneNumber = "01129085446"
                    };

                    await userManager.CreateAsync(Admin, "P@ss0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    logger.LogInformation("Identity data seeding completed successfully.");
                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity seeding Failed");
            }
        }
    }
}
