﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Developer.Models;
using AiursoftBase.Attributes;
using AiursoftBase;
using AiursoftBase.Models.Developer;
using AiursoftBase.Models;

namespace Developer.Controllers
{
    [AiurExceptionHandler]
    [AiurRequireHttps]
    public class HomeController : AiurController
    {
        public readonly SignInManager<DeveloperUser> _signInManager;
        public readonly ILogger _logger;

        public HomeController(
            SignInManager<DeveloperUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        [AiurForceAuth(preferController: "Apps", preferAction: "Index", justTry: true)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Docs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return SignoutRootServer(new AiurUrl(string.Empty, "Home", nameof(HomeController.Index), new { }));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
