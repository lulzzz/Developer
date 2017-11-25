using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Developer;
using Aiursoft.Pylon.Models.Developer.ApiAddressModels;
using Aiursoft.Pylon.Models.Developer.ApiViewModels;
using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [AiurRequireHttps]
    public class ApiController : AiurApiController
    {
        public readonly UserManager<DeveloperUser> _userManager;
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly ILogger _logger;
        public DeveloperDbContext _dbContext;

        public ApiController(
        UserManager<DeveloperUser> userManager,
        SignInManager<DeveloperUser> signInManager,
        ILoggerFactory loggerFactory,
        DeveloperDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ApiController>();
            _dbContext = _context;
        }

        [AiurExceptionHandler]
        public async Task<JsonResult> IsValidApp(IsValidateAppAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AiurProtocal { message = "Wrong input.", code = ErrorType.InvalidInput });
            }
            var _target = await _dbContext.Apps.FindAsync(model.AppId);
            if (_target == null)
            {
                return Json(new AiurProtocal { message = "Target app did not found.", code = ErrorType.NotFound });
            }
            else if (_target.AppSecret != model.AppSecret)
            {
                return Json(new AiurProtocal { message = "Wrong secret.", code = ErrorType.WrongKey });
            }
            else
            {
                return Json(new AiurProtocal { message = "Correct app info.", code = ErrorType.Success });
            }
        }
        //http://developer.aiursoft.obisoft.com.cn/api/AppInfo?AppId=29bf5250a6d93d47b6164ac2821d5009
        [AiurExceptionHandler]
        public async Task<JsonResult> AppInfo(AppInfoAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AiurProtocal { message = "Wrong input.", code = ErrorType.InvalidInput });
            }
            var permissions = await _dbContext
                .AppPermissions
                .Include(t => t.Permission)
                .Where(t => t.AppId == model.AppId)
                .ToListAsync();

            var target = await _dbContext
                .Apps
                .SingleOrDefaultAsync(t => t.AppId == model.AppId);

            if (target == null)
            {
                return Json(new AiurProtocal { message = "Not found.", code = ErrorType.NotFound });
            }
            target.Permissions = permissions;
            return Json(new AppInfoViewModel
            {
                message = "Successfully get target app info.",
                code = ErrorType.Success,
                App = target
            });
        }
    }
}