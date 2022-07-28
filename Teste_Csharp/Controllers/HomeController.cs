using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Teste_Csharp.Models;

namespace Teste_Csharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public UserManager<MyUser> UserManager { get; }

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(obj.UserName);
                if (user != null && await  _userManager.CheckPasswordAsync(user,obj.Password))
                {
                    var identity = new ClaimsIdentity("cookies");
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

                    await HttpContext.SignInAsync("cookies", new ClaimsPrincipal(identity));
                    return RedirectToAction("About");
                }
                ModelState.AddModelError("", "Usuário ou Senha Invalida");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(obj.UserName);
                if(user == null)
                {
                    user = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = obj.UserName
                    };
                    var result = await _userManager.CreateAsync(user, obj.Password);
                }
                return View("Success");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
