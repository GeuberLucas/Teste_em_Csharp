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
        private readonly UserManager<MyUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<MyUser> _userClaimsPrincipalFactory;
        private readonly SignInManager<MyUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<MyUser> userManager, IUserClaimsPrincipalFactory<MyUser> userClaimsPrincipalFactory,
            SignInManager<MyUser> signInManager )
        {
            _logger = logger;
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _signInManager = signInManager;
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
                if (user != null && !await _userManager.IsLockedOutAsync(user))
                {

                    if(await _userManager.CheckPasswordAsync(user, obj.Password))
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError("", "Email Invalida");
                            return View();
                        }
                        await _userManager.ResetAccessFailedCountAsync(user);

                        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

                        await HttpContext.SignInAsync("Identity.Application", principal);
                        var signInResult = await _signInManager.PasswordSignInAsync(obj.UserName, obj.Password, false, false);

                        if (signInResult.Succeeded)
                        {
                            return RedirectToAction("About");
                        }
                    }
                    await _userManager.AccessFailedAsync(user);

                    if(await _userManager.IsLockedOutAsync(user))
                    {

                    }
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
                    user = new MyUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = obj.UserName, 
                        Email = obj.UserName
                    };
                    
        
                    var result = await _userManager.CreateAsync(user, obj.Password);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var conf = Url.Action("ConfirmeEmailAddress", "Home",
                           new { token = token, email = user.Email }, Request.Scheme);
                        System.IO.File.WriteAllText("resetLink.txt", conf);
                        return View("Success");
                    }
                    else
                    {
                        foreach(var erro in result.Errors)
                        {
                            ModelState.AddModelError("", erro.Description);
                        }

                        return View();
                    }
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
        public async Task<IActionResult> ConfirmeEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {                    
                    return View("Success");
                }
            }
                return View("Error");
        }


        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(obj.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action("ResetPassword","Home",new { token = token, email = obj.Email }, Request.Scheme );

                    System.IO.File.WriteAllText("resetLink.txt", resetUrl);
                    return View("Success");
                }
                else
                {
                    ModelState.AddModelError("", "Usuário Invalido");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(obj.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, obj.Token, obj.Passord);
                    if (!result.Succeeded)
                    {
                        foreach(var erro in result.Errors)
                        {

                            ModelState.AddModelError("", erro.Description);
                        }
                        return View();
                    }
                    return View("Success");
                }
                ModelState.AddModelError("", "Invalid Request");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token,string email)
        {
            return View(new ResetPasswordModel { Token= token, Email=email} );
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
