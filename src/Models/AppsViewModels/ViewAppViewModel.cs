﻿using Aiursoft.Pylon;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.OSS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services.ToOSSServer;
using Aiursoft.Pylon.Models.API;
using Aiursoft.Developer.Data;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.Developer.Models.AppsViewModels
{
    public class ViewAppViewModel : CreateAppViewModel
    {
        [Obsolete(message: "This method is only for framework", error: true)]
        public ViewAppViewModel() { }
        public static async Task<ViewAppViewModel> SelfCreateAsync(DeveloperUser User, App ThisApp, DeveloperDbContext _dbContext)
        {
            var model = new ViewAppViewModel(User, ThisApp);
            await model.Recover(User, ThisApp, _dbContext);
            return model;
        }

        public async Task Recover(DeveloperUser User, App ThisApp, DeveloperDbContext _dbContext)
        {
            base.Recover(User, 1);
            var token = AppsContainer.AccessToken(ThisApp.AppId, ThisApp.AppSecret);

            var buckets = await ApiService.ViewMyBucketsAsync(await token());
            Buckets = buckets.Buckets;

            var grants = await Pylon.Services.ToAPIServer.ApiService.AllUserGrantedAsync(await token());
            Grants = grants.Grants;
        }

        private ViewAppViewModel(DeveloperUser User, App ThisApp) : base(User)
        {
            if (ThisApp.CreaterId != User.Id)
            {
                throw new InvalidOperationException("The app is not the user's app!");
            }
            AppName = ThisApp.AppName;
            AppDescription = ThisApp.AppDescription;
            AppCategory = ThisApp.AppCategory;
            AppPlatform = ThisApp.AppPlatform;
            AppId = ThisApp.AppId;
            AppSecret = ThisApp.AppSecret;
            EnableOAuth = ThisApp.EnableOAuth;
            ForceInputPassword = ThisApp.ForceInputPassword;
            ForceConfirmation = ThisApp.ForceConfirmation;
            DebugMode = ThisApp.DebugMode;
            PrivacyStatementUrl = ThisApp.PrivacyStatementUrl;
            LicenseUrl = ThisApp.LicenseUrl;
            AppIconAddress = ThisApp.AppIconAddress;
            AppDomain = ThisApp.AppDomain;
            ViewOpenId = ThisApp.ViewOpenId;
            ViewPhoneNumber = ThisApp.ViewPhoneNumber;
            ChangePhoneNumber = ThisApp.ChangePhoneNumber;
            ConfirmEmail = ThisApp.ConfirmEmail;
            ChangeBasicInfo = ThisApp.ChangeBasicInfo;
            ChangePassword = ThisApp.ChangePassword;
        }

        public virtual bool JustHaveUpdated { get; set; } = false;
        public virtual string AppId { get; set; }
        public virtual string AppSecret { get; set; }
        public virtual string AppIconAddress { get; set; }
        [Url]
        [Display(Name = "Privacy Statement Url")]
        public virtual string PrivacyStatementUrl { get; set; }
        [Url]
        [Display(Name = "License Url")]
        public virtual string LicenseUrl { get; set; }

        [Display(Name = "Enable OAuth")]
        public virtual bool EnableOAuth { get; set; }
        [Display(Name = "Force Input Password")]
        public virtual bool ForceInputPassword { get; set; }
        [Display(Name = "Force Confirmation")]
        public virtual bool ForceConfirmation { get; set; }
        [Display(Name = "Debug Mode")]
        public virtual bool DebugMode { get; set; }
        [Display(Name = "App Domain")]
        public virtual string AppDomain { get; set; }
        // Permissions
        [Display(Name = "View open Id")]
        public bool ViewOpenId { get; set; } = true;
        [Display(Name = "View phone number")]
        public bool ViewPhoneNumber { get; set; }
        [Display(Name = "Change phone number")]
        public bool ChangePhoneNumber { get; set; }
        [Display(Name = "Change Email confirmation status")]
        public bool ConfirmEmail { get; set; }
        [Display(Name = "Change basic info")]
        public bool ChangeBasicInfo { get; set; }
        [Display(Name = "Change password")]
        public bool ChangePassword { get; set; }

        public IEnumerable<Bucket> Buckets { get; set; } //= new List<Bucket>();
        public IEnumerable<Grant> Grants { get; set; }
        public IEnumerable<ViewAblePermission> ViewAblePermission { get; set; }
    }

    public class ViewAblePermission
    {
        public virtual int PermissionId { get; set; }
        public virtual string PermissionName { get; set; }
        public virtual bool Permitted { get; set; }
    }
}
