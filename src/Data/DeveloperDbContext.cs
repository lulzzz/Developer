using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Developer.Models;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Builder;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.Developer.Data
{
    public class DeveloperDbContext : IdentityDbContext<DeveloperUser>
    {
        public DeveloperDbContext(DbContextOptions<DeveloperDbContext> options)
            : base(options)
        {
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<AppPermission> AppPermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public void Seed()
        {
            if (Permissions.Count() == 0)
            {
                Permissions.Add(new Permission { PermissionName = "View your basic identity info like ID and Email." });
                Permissions.Add(new Permission { PermissionName = "View your phone number." });
                Permissions.Add(new Permission { PermissionName = "Change your phone number." });
                Permissions.Add(new Permission { PermissionName = "Change your basic info like nickname." });
                Permissions.Add(new Permission { PermissionName = "Change your password with your old password." });
            }
            SaveChanges();
        }
    }
}
