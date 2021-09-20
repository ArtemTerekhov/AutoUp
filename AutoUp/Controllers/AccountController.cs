using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoUp.Models;
using AutoUp.ViewModels;


namespace AutoUpApp.Controllers
{
    public class AccountController : Controller
    {
        private AutoUpContext db;
        public AccountController(AutoUpContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);


                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Forum");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

                Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");

                if (user == null)
                {
                    db.Users.Add(new User
                    {
                        Login = model.Login,
                        Password = model.Password,
                        Email = model.Email,
                        Telegram = model.Telegram,
                        Jabber = model.Jabber,
                        Balance = 0.00M,
                        Role = userRole

                    });
                    await db.SaveChangesAsync();
                    User newUser = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

                    await Authenticate(newUser); // аутентификация

                    return RedirectToAction("Login", "Account");
                }
                else
                    // ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                    await Authenticate(user); // аутентификация

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ShowUserProfile(string login)
        {
            if (!String.IsNullOrEmpty(login))
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == login);

                return View(user);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword(int? userId)
        {
            if (userId != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                ChangePassViewModel model = new ChangePassViewModel
                {
                    UserId = user.UserId,
                    ActualPassword = user.Password
                };

                return View(model);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == changePassword.UserId);

                user.Password = changePassword.Password;

                db.Users.Update(user);
                await db.SaveChangesAsync();

                return RedirectToAction("ShowUserProfile", "Account", new { login = user.Login });
            }

            return NotFound();
        }

        [Authorize]
        public IActionResult BankAccountReplenishment()
        {
            return View();
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var users = await db.Users.ToListAsync();

            foreach (User user in users)
            {
                if (email == user.Email)
                {
                    return Json(false);
                }
            }
            return Json(true);
        }
    }
}