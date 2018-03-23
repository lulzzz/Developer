﻿using System;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
