using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Developer.Data;
using Developer.Models;
using AiursoftBase;
using AiursoftBase.Models.Developer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Developer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public bool IsDevelopment { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            IsDevelopment = env.IsDevelopment();
            if (IsDevelopment)
            {
                Values.Schema = "http";
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConnectToAiursoftDatabase<DeveloperDbContext>("Developer", IsDevelopment);
            services.AddIdentity<DeveloperUser, IdentityRole>()
                .AddEntityFrameworkStores<DeveloperDbContext>()
                .AddDefaultTokenProviders();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DeveloperDbContext dbContext)
        {
            var SupportedCultures = new CultureInfo[]
            {
                new CultureInfo("en"),
                new CultureInfo("zh")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = SupportedCultures,
                SupportedUICultures = SupportedCultures
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseAuthentication()
            .UseAiursoftAuthentication(appId: "29bf5250a6d93d47b6164ac2821d5009", appSecret: "784400c3d9066c5584489497273f867e")
            .UseMvcWithDefaultRoute();
        }
    }
}
