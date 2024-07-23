using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TTProject.Core.Entities;

namespace TTProject.Infrastructure
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole<long>> roleManager)
        {
            // Assurez-vous que les rôles existent
            string[] roles = { "ProjectManager", "Employee" };
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole<long>(role));
                }
            }
        }
    }
}
