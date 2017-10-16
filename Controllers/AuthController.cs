using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Developer.Models;
using Developer.Data;
using AiursoftBase.Services;
using AiursoftBase.Attributes;
using System;
using AiursoftBase.Models;
using AiursoftBase.Models.ForApps.AddressModels;
using AiursoftBase;
using AiursoftBase.Models.Developer;

namespace Developer.Controllers
{
    [AiurExceptionHandler]
    [AiurRequireHttps]
    public class AuthController : AiurController
    {
        public readonly UserManager<DeveloperUser> _userManager;
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly DeveloperDbContext _dbContext;

        public AuthController(
            UserManager<DeveloperUser> userManager,
            SignInManager<DeveloperUser> signInManager,
            DeveloperDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = _context;
        }

        [AiurForceAuth(preferController: "Apps", preferAction: "Index", justTry: false)]
        public IActionResult GoAuth()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> AuthResult(AuthResultAddressModel model)
        {
            await AuthProcess.AuthApp(this, model, _userManager, _signInManager);
            return Redirect(model.state);
        }
    }
}