using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Developer.Models;
using AiursoftBase.Models;
using Microsoft.AspNetCore.Builder;
using AiursoftBase.Models.Developer;

namespace Developer.Data
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
    }
}
