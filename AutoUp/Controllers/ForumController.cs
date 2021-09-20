using System;
using System.Collections.Generic;
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
    public class ForumController : Controller
    {
        private readonly AutoUpContext db;
        private readonly Dictionary<string, string> ud = new Dictionary<string, string>()
            {
                { "60", "60 секунд" },
                { "120", "120 секунд" },
                { "240", "240 секунд" },
                { "300", "300 секунд" }
            };

        private readonly Dictionary<string, string> ls = new Dictionary<string, string>()
            {
                { "true", "Активна"},
                { "false", "Неактивна"},
            };

        public ForumController(AutoUpContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(int? forumId, string name, int page = 1)
        {
            int pageSize = 5;
            int count = 0;
            var items = new List<ForumLink>();

            //фильтрация
            IQueryable<Forum> forums = db.Forums;

            if (forumId != null && forumId != 0)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                int userId = user.UserId;

                IQueryable<ForumLink> forumLinks = db.ForumLinks;
                forumLinks = forumLinks.Where(fl => fl.UserId == userId && fl.ForumId == forumId);

                // пагинация
                count = await forumLinks.CountAsync();
                items = await forumLinks.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }

            ForumViewModel viewModel = new ForumViewModel
            {
                PageViewModel = new PageViewModel(count-1, page, pageSize),
                FilterForumViewModel = new FilterForumViewModel(db.Forums.ToList(), forumId, name),
                ForumLink = items,
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateLink(int? forumId)
        {
            if (forumId != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                int userId = user.UserId;
                string balance = user.Balance.ToString();

                ForumLinkViewModel viewModel = new ForumLinkViewModel
                {
                    ForumId = Convert.ToInt32(forumId),
                    UserId = userId,
                    Balance = balance
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLink(ForumLinkViewModel model)
        {
            if (model != null)
            {
                ViewBag.UpDelays = ud;

                ViewBag.LinkStates = ls;

                ForumLink forumLink = new ForumLink
                {
                    UserId = model.UserId,
                    ForumId = model.ForumId,
                    Login = model.Login,
                    Password = model.Password,
                    SecretWord = model.SecretWord,
                    LinkUrl = model.LinkUrl,
                    ForumDelay = Convert.ToInt16(model.ForumDelay),
                    LinkState = Convert.ToBoolean(model.LinkState)
                };

                db.ForumLinks.Add(forumLink);

                await db.SaveChangesAsync();

                // int newForumLinkId = forumLink.ForumLinkId;

                return RedirectToAction("Index", new { forumId = model.ForumId });
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? forumLinkId)
        {
            if (forumLinkId != null)
            {
                ViewBag.UpDelays = ud;

                ViewBag.LinkStates = ls;

                ForumLink forumLink = await db.ForumLinks.FirstOrDefaultAsync(fl => fl.ForumLinkId == forumLinkId);

                var model = new ForumLinkViewModel
                {
                    ForumLinkId = forumLinkId,
                    UserId = forumLink.UserId,
                    ForumId = forumLink.ForumId,
                    Login = forumLink.Login,
                    Password = forumLink.Password,
                    SecretWord = forumLink.SecretWord,
                    LinkUrl = forumLink.LinkUrl,
                    ForumDelay = forumLink.ForumDelay.ToString(),
                    LinkState = forumLink.LinkState.ToString()
                };

                return View(model);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ForumLinkViewModel model)
        {
            if (model != null)
            {
                ViewBag.UpDelays = ud;

                ViewBag.LinkStates = ls;

                ForumLink forumLink = new ForumLink
                {
                    ForumLinkId = Convert.ToInt32(model.ForumLinkId),
                    UserId = model.UserId,
                    ForumId = model.ForumId,
                    Login = model.Login,
                    Password = model.Password,
                    SecretWord = model.SecretWord,
                    LinkUrl = model.LinkUrl,
                    ForumDelay = Convert.ToInt16(model.ForumDelay),
                    LinkState = Convert.ToBoolean(model.LinkState)
                };

                db.ForumLinks.Update(forumLink);

                await db.SaveChangesAsync();

                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? forumLinkId)
        {
            if (forumLinkId != null)
            {
                ForumLink forumLink = await db.ForumLinks.FirstOrDefaultAsync(fl => fl.ForumLinkId == forumLinkId);

                if (forumLink != null)
                {
                    db.ForumLinks.Remove(forumLink);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", new { forumId = forumLink.ForumId });
                }
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> GetStatistics(int page = 1,
        SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 5;

            User user = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            int userId = user.UserId;
            string balance = user.Balance.ToString();

            List<Forum> forums = await db.Forums.ToListAsync();
            Dictionary<int, List<ForumLink>> forumLinks = new Dictionary<int, List<ForumLink>>();
            Dictionary<int, List<ForumTime>> forumTimes = new Dictionary<int, List<ForumTime>>();

            int forumsNumber = 0;

            foreach (Forum forum in forums)
            {
                List<ForumLink> forumLinksSet = await db.ForumLinks
                   .Where(fl => fl.ForumId == forum.ForumId && fl.UserId == userId).ToListAsync();
                forumLinks.Add(forum.ForumId, forumLinksSet);

                List<ForumTime> forumTimesSet = await db.ForumTimes
                    .Where(ts => ts.ForumId == forum.ForumId && ts.UserId == userId).ToListAsync();
                forumTimes.Add(forum.ForumId, forumTimesSet);

                if (forumLinksSet.Count > 0)
                {
                    forumsNumber += 1;
                }

            };

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    forums = forums.OrderByDescending(f => f.Name).ToList();
                    break;
                default:
                    forums = forums.OrderBy(f => f.Name).ToList();
                    break;
            }

            var count = forumsNumber;
            var items = forums.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            StatisticsViewModel viewModel = new StatisticsViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortForumViewModel = new SortForumViewModel(sortOrder),
                Forums = forums,
                ForumLinks = forumLinks,
                ForumTimes = forumTimes,
                Balance = balance
            };

            return View(viewModel);
        }
    }
}