using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoUp.Enums;
using AutoUp.Models;
using AutoUp.ViewModels;

namespace AutoUp.Controllers
{
    public class AdminController : Controller
    {
        private AutoUpContext db;

        public AdminController(AutoUpContext context)
        {
            db = context;

            if (db.Forums.Count() == 0)
            {
                Forum two = new Forum { Name = "BDF", Url = "https://bdf-club.com/", UpPrice = 0.0M };
                Forum three = new Forum { Name = "BHF", Url = "https://bhf.io/", UpPrice = 0.0M };
                Forum four = new Forum { Name = "CENTER", Url = "https://center-club.us/", UpPrice = 0.0M };
                Forum five = new Forum { Name = "CLUB2CRD", Url = "https://crdclub.su/", UpPrice = 0.0M };
                Forum seven = new Forum { Name = "Deep Web", Url = "https://search.deepweb.to/", UpPrice = 0.0M };
                Forum eight = new Forum { Name = "DUBLICAT", Url = "https://www.dublikat.shop/", UpPrice = 0.0M };
                Forum ten = new Forum { Name = "OpenCard", Url = "https://opencard.us/", UpPrice = 0.0M };
                Forum eleven = new Forum { Name = "Openssource", Url = "https://openssource.info/", UpPrice = 0.0M };
                Forum thirteen = new Forum { Name = "ProCrd", Url = "https://procrd.net/", UpPrice = 0.0M };
                Forum fourteen = new Forum { Name = "SkyNetZone", Url = "https://skynetzone.pw/", UpPrice = 0.0M };
                Forum fifteen = new Forum { Name = "TENEC", Url = "https://tenec.to/", UpPrice = 0.0M };
                Forum sixteen = new Forum { Name = "WWHClub", Url = "https://wwh-club.net/", UpPrice = 0.0M };
                Forum seventeen = new Forum { Name = "XSS", Url = "https://xss.is/", UpPrice = 0.0M };
                Forum eighteen = new Forum { Name = "MONEYMAKER", Url = "https://moneymaker.hk/", UpPrice = 0.0M };

                db.Forums.AddRange(two, three, four, five, seven, eight, ten, eleven,
                              thirteen, fourteen, fifteen, sixteen, seventeen, eighteen);
                db.SaveChanges();

            }
        }

        [Authorize]
        public async Task<IActionResult> Index(int? forum, string name, int page = 1,
            SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 5;

            //фильтрация
            IQueryable<Forum> forums = db.Forums;

            if (forum != null && forum != 0)
            {
                forums = forums.Where(f => f.ForumId == forum);
            }
            if (!String.IsNullOrEmpty(name))
            {
                forums = forums.Where(f => f.Name.Contains(name));
            }

            // сортировка
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    forums = forums.OrderByDescending(f => f.Name);
                    break;
                case SortState.UpAsc:
                    forums = forums.OrderBy(f => f.UpPrice);
                    break;
                case SortState.UpDesc:
                    forums = forums.OrderByDescending(f => f.UpPrice);
                    break;
                case SortState.UrlAsc:
                    forums = forums.OrderBy(f => f.Url);
                    break;
                case SortState.UrlDesc:
                    forums = forums.OrderByDescending(f => f.Url);
                    break;
                default:
                    forums = forums.OrderBy(f => f.Name);
                    break;
            }

            // пагинация
            var count = await forums.CountAsync();
            var items = await forums.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexForumViewModel viewModel = new IndexForumViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortForumViewModel = new SortForumViewModel(sortOrder),
                FilterForumViewModel = new FilterForumViewModel(db.Forums.ToList(), forum, name),
                Forums = items
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditForumViewModel forum)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");

                Forum forumModel = new Forum
                {
                    Name = forum.Name,
                    Url = forum.Url,
                    UpPrice = Convert.ToDecimal(forum.UpPrice, culture),
                    UpTime = forum.UpTime
                };

                db.Forums.Add(forumModel);
                await db.SaveChangesAsync();

                int newForumId = forumModel.ForumId;

                var currentTime = DateTime.Now;

                foreach (string forumTime in forum.DummyForumTimes)
                {
                    DateTime newForumDateTime;

                    var forumDateTime = Convert.ToDateTime(forumTime);

                    if (DateTime.Compare(forumDateTime, currentTime) <= 0)
                    {
                        newForumDateTime = forumDateTime.AddDays(1);
                        forumDateTime = newForumDateTime;
                    }

                    var linkUpDate = forumDateTime.ToString("ddMMyy");

                    db.ForumTimes.Add(new ForumTime
                    {
                        ForumId = newForumId,
                        UserId = 1,
                        DateTime = forumTime,
                        Date = linkUpDate
                    }); 

                    await db.SaveChangesAsync();
                };

                if (forum.UpTime != null && forum.UpTime != string.Empty)
                {
                    DateTime newForumDateTime;

                    var forumDateTime = Convert.ToDateTime(forum.UpTime);

                    if (DateTime.Compare(forumDateTime, currentTime) <= 0)
                    {
                        newForumDateTime = forumDateTime.AddDays(1);
                        forumDateTime = newForumDateTime;
                    }

                    var linkUpDate = forumDateTime.ToString("ddMMyy");

                    db.ForumTimes.Add(new ForumTime
                    {
                        ForumId = forum.ForumId,
                        UserId = 1,
                        DateTime = forum.UpTime,
                        Date = linkUpDate
                    });

                    await db.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? forumId)
        {
            if (forumId != null)
            {
                Forum forum = await db.Forums.FirstOrDefaultAsync(f => f.ForumId == forumId);
                List<ForumTime> forumTimeObjects = await db.ForumTimes.Where(ft => ft.ForumId == forumId).ToListAsync();
                var forumTimesList = new List<string>();

                foreach (var forumTimeObject in forumTimeObjects)
                {
                    forumTimesList.Add(forumTimeObject.DateTime);
                }

                var forumTimes = forumTimesList.ToArray();

                EditForumViewModel model = new EditForumViewModel
                {
                    ForumId = forum.ForumId,
                    Name = forum.Name,
                    Url = forum.Url,
                    UpPrice = forum.UpPrice.ToString().Replace(",", "."),
                    UpTime = forum.UpTime,
                    DummyForumTimes = forumTimes
                };

                return View(model);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditForumViewModel forum)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");

                Forum forumModel = new Forum
                {
                    ForumId = forum.ForumId,
                    Name = forum.Name,
                    Url = forum.Url,
                    UpPrice = Convert.ToDecimal(forum.UpPrice, culture),
                    UpTime = forum.UpTime
                };

                db.Forums.Update(forumModel);
                await db.SaveChangesAsync();
                List<ForumTime> forumTimeObjects = await db.ForumTimes.Where(ft => ft.ForumId == forum.ForumId).ToListAsync();

                foreach (var forumTimeObject in forumTimeObjects)
                {
                    db.ForumTimes.Remove(forumTimeObject);
                    await db.SaveChangesAsync();
                }

                var currentTime = Convert.ToDateTime(DateTime.Now); 

                if (forum.DummyForumTimes != null)
                {
                    foreach (string forumTime in forum.DummyForumTimes)
                    {
                        DateTime newForumDateTime;

                        var forumDateTime = Convert.ToDateTime(forumTime);

                        if (DateTime.Compare(forumDateTime, currentTime) <= 0)
                        {
                            newForumDateTime = forumDateTime.AddDays(1);
                            forumDateTime = newForumDateTime;
                        }

                        var linkUpDate = forumDateTime.ToString("ddMMyy");

                        db.ForumTimes.Add(new ForumTime
                        {
                            ForumId = forum.ForumId,
                            UserId = 1,
                            DateTime = forumTime,
                            Date = linkUpDate
                        }); 

                        await db.SaveChangesAsync();
                    };
                }

                if (forum.UpTime != null && forum.UpTime != string.Empty)
                {
                    DateTime newForumDateTime;

                    var isInArray = false;

                    if (forum.DummyForumTimes != null)
                    {

                        foreach (string forumTime in forum.DummyForumTimes)
                        {
                            if (forumTime == forum.UpTime)
                            {
                                isInArray = true;
                            }
                        }

                    }

                    if (isInArray == false)
                    {

                        var forumDateTime = Convert.ToDateTime(forum.UpTime);

                        if (DateTime.Compare(forumDateTime, currentTime) <= 0)
                        {
                            newForumDateTime = forumDateTime.AddDays(1);
                            forumDateTime = newForumDateTime;
                        }

                        var linkUpDate = forumDateTime.ToString("ddMMyy");

                        db.ForumTimes.Add(new ForumTime
                        {
                            ForumId = forum.ForumId,
                            UserId = 1,
                            DateTime = forum.UpTime,
                            Date = linkUpDate
                        });

                        await db.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? forumId)
        {
            if (forumId != null)
            {
                Forum forum = await db.Forums.FirstOrDefaultAsync(f => f.ForumId == forumId);

                if (forum != null)
                {
                    db.Forums.Remove(forum);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        [Authorize]
        public async new Task<IActionResult> User(int? user, string login, int page = 1,
           UserSortState sortOrder = UserSortState.LoginAsc)
        {
            int pageSize = 5;

            //фильтрация
            IQueryable<User> users = db.Users.Include(u => u.Role);

            if (user != null && user != 0)
            {
                users = users.Where(u => u.UserId == user);
            }
            if (!String.IsNullOrEmpty(login))
            {
                users = users.Where(u => u.Login.Contains(login));
            }

            // сортировка
            switch (sortOrder)
            {
                case UserSortState.LoginDesc:
                    users = users.OrderByDescending(u => u.Login);
                    break;
                case UserSortState.EmailAsc:
                    users = users.OrderBy(u => u.Email);
                    break;
                case UserSortState.EmailDesc:
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case UserSortState.BalanceAsc:
                    users = users.OrderBy(u => u.Balance);
                    break;
                case UserSortState.BalanceDesc:
                    users = users.OrderByDescending(u => u.Balance);
                    break;
                case UserSortState.RoleAsc:
                    users = users.OrderBy(u => u.Role.Name);
                    break;
                case UserSortState.RoleDesc:
                    users = users.OrderByDescending(u => u.Role.Name);
                    break;
                default:
                    users = users.OrderBy(u => u.Login);
                    break;
            }

            // пагинация
            var count = await users.CountAsync();
            var items = await users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexUserViewModel viewModel = new IndexUserViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortUserViewModel = new SortUserViewModel(sortOrder),
                FilterUserViewModel = new FilterUserViewModel(db.Users.ToList(), user, login),
                Users = items
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int? userId)
        {
            if (userId != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("User");
                }
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditUser(int? userId)
        {
            if (userId != null)
            {
                User user = await db.Users.Include(u => u.Role)
                                  .FirstOrDefaultAsync(u => u.UserId == userId);

                int role = user.Role.RoleId;
                string name = user.Role.Name;

                EditUserViewModel model = new EditUserViewModel
                {
                    UserId = user.UserId,
                    Login = user.Login,
                    Password = user.Password,
                    ConfirmPassword = user.Password,
                    Email = user.Email,
                    Telegram = user.Telegram,
                    Jabber = user.Jabber,
                    Balance = user.Balance.ToString().Replace(",", "."),
                    FilterRoleViewModel = new FilterRoleViewModel(db.Roles.ToList(), role, name)
                };

                return View(model);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel user, int role)
        {
            if (ModelState.IsValid)
            {
                CultureInfo culture = new CultureInfo("en-US");

                User userModel = new User
                {
                    UserId = user.UserId,
                    Login = user.Login,
                    Password = user.Password,
                    Email = user.Email,
                    Telegram = user.Telegram,
                    Jabber = user.Jabber,
                    RoleId = role,
                    Balance = Convert.ToDecimal(user.Balance, culture)
                };

                db.Users.Update(userModel);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("User");
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
