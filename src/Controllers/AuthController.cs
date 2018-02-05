using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Developer.Models;
using Aiursoft.Developer.Data;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Attributes;
using System;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.ForApps.AddressModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models.Developer;

namespace Aiursoft.Developer.Controllers
{
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

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: false)]
        public IActionResult GoAuth()
        {
            throw new NotImplementedException();
        }

        [AiurForceAuth(preferController: "", preferAction: "", justTry: false, register: true)]
        public IActionResult GoRegister()
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