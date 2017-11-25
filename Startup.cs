﻿using System;
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
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Aiursoft.Developer
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
            var developerAppId = Configuration["developerAppId"];
            var developerAppSecret = Configuration["developerAppSecret"];
            if (string.IsNullOrWhiteSpace(developerAppId) || string.IsNullOrWhiteSpace(developerAppSecret))
            {
                throw new InvalidOperationException("Did not get appId and appSecret from configuration!");
            }
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
            .UseAiursoftAuthentication(appId: developerAppId, appSecret: developerAppSecret)
            .UseMvcWithDefaultRoute();
        }
    }
}
