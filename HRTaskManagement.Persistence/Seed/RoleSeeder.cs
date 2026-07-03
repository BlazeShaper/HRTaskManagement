// Persistence/Seed/RoleSeeder.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;
using HRTaskManagement.Shared.Constants;

namespace HRTaskManagement.Persistence.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(WorkSphereDbContext context)
        {
            foreach (var roleName in SystemRoles.All)
            {
                bool exists = await context.Roles
                    .AnyAsync(r => r.Name == roleName);

                if (!exists)
                {
                    context.Roles.Add(new Role
                    {
                        Name = roleName,
                        Description = $"Sistem varsayılan rolü: {roleName}"
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}