using AiursoftBase;
using AiursoftBase.Attributes;
using AiursoftBase.Models;
using AiursoftBase.Models.Developer;
using AiursoftBase.Services;
using AiursoftBase.Services.ToOSSServer;
using Developer.Data;
using Developer.Models;
using Developer.Models.AppsViewModels;
using Developer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Developer.Controllers
{
    [AiurForceAuth]
    [AiurExceptionHandler]
    [AiurRequireHttps]
    public class AppsController : AiurController
    {
        private readonly UserManager<DeveloperUser> _userManager;
        private readonly SignInManager<DeveloperUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly DeveloperDbContext _dbContext;

        public AppsController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AppsController>();
            _dbContext = _context;
        }

        public async Task<IActionResult> Index()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new IndexViewModel(cuser);
            return View(model);
        }

        public async Task<IActionResult> AllApps()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new AllAppsViewModel(cuser)
            {
                AllApps = _dbContext.Apps.Where(t => t.CreaterId == cuser.Id)
            };
            return View(model);
        }

        public async Task<IActionResult> CreateApp()
        {
            var cuser = await GetCurrentUserAsync();
            var model = new CreateAppViewModel(cuser);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApp(CreateAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                model.Recover(cuser, 1);
                return View(model);
            }
            string iconPath = string.Empty;
            if (Request.Form.Files.Count == 0 || Request.Form.Files.First().Length < 1)
            {
                iconPath = Values.DeveloperServerAddress + "/images/appdefaulticon.png";
            }
            else
            {
                iconPath = await StorageService.SaveToOSS(Request.Form.Files.First(), Values.AppsIconBucketId);
            }

            var _newApp = new App(cuser.Id, model.AppName, model.AppDescription, model.AppCategory, model.AppPlatform)
            {
                CreaterId = cuser.Id,
                AppIconAddress = iconPath
            };
            _dbContext.Apps.Add(_newApp);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = _newApp.AppId });
        }

        public async Task<IActionResult> ViewApp(string id, bool JustHaveUpdated = false)
        {
            var app = await _dbContext.Apps.FindAsync(id);
            if (app == null)
            {
                return NotFound();
            }
            var cuser = await GetCurrentUserAsync();
            var model = await ViewAppViewModel.SelfCreateAsync(cuser, app, _dbContext);
            model.JustHaveUpdated = JustHaveUpdated;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ViewApp(ViewAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                await model.Recover(cuser, await _dbContext.Apps.FindAsync(model.AppId), _dbContext);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            target.AppName = model.AppName;
            target.AppDescription = model.AppDescription;
            target.EnableOAuth = model.EnableOAuth;
            target.ForceInputPassword = model.ForceInputPassword;
            target.ForceConfirmation = model.ForceConfirmation;
            target.DebugMode = model.DebugMode;
            target.PrivacyStatementUrl = model.PrivacyStatementUrl;
            target.LicenseUrl = model.LicenseUrl;
            target.AppDomain = model.AppDomain;
            _dbContext.AppPermissions.Delete(t => t.AppId == target.AppId);
            foreach (var key in HttpContext.Request.Form.Keys)
            {
                if (key.StartsWith("PermissionStatus") && HttpContext.Request.Form[key] == "on")
                {
                    var pId = Convert.ToInt32(key.Substring("PermissionStatus".Length));
                    _dbContext.AppPermissions.Add(new AppPermission
                    {
                        AppId = target.AppId,
                        PermissionId = pId
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewApp), new { id = target.AppId, JustHaveUpdated = true });
        }

        public async Task<IActionResult> DeleteApp(string id)
        {
            var cuser = await GetCurrentUserAsync();
            var _target = await _dbContext.Apps.FindAsync(id);
            if (_target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            var model = new DeleteAppViewModel(cuser)
            {
                AppId = _target.AppId,
                AppName = _target.AppName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(DeleteAppViewModel model)
        {
            var cuser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.Recover(cuser, 1);
                return View(model);
            }
            var target = await _dbContext.Apps.FindAsync(model.AppId);
            if (target == null)
            {
                return NotFound();
            }
            else if (target.CreaterId != cuser.Id)
            {
                return new UnauthorizedResult();
            }
            await ApiService.DeleteAppAsync(await AppsContainer.AccessToken(target.AppId, target.AppSecret)(), target.AppId);
            _dbContext.Apps.Remove(target);
            _dbContext.AppPermissions.Delete(t => t.AppId == target.AppId);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(AllApps));

        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeIcon(string AppId)
        {
            if (Request.Form.Files.Count != 0 && Request.Form.Files.First().Length > 1)
            {
                var appExists = await _dbContext.Apps.FindAsync(AppId);
                appExists.AppIconAddress = await StorageService.SaveToOSS(Request.Form.Files.First(), Values.AppsIconBucketId);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ViewApp), new { id = AppId, JustHaveUpdated = true });
        }
    }
}
