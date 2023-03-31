using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Drawing.Printing;
using System.Security.AccessControl;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int? page, string name, int tagId, int categoryId)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (accClaim != null)
            {
                ViewBag.CurrentUsername = accClaim.Value;
                var accId = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().AccountId;
                var cartCount = _dbContext.Carts
                    .Where(x => x.AccountId == accId)
                    .ToList();
                ViewBag.CartCount = cartCount.Count();
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            int pageSize = 3;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var query = _dbContext.News.AsQueryable();
            if (name != null)
            {
                query = query.Where(x => x.Title.ToLower().Contains(name.ToLower()));
            }

            if (tagId > 0)
            {
                query = from x in query join nt in _dbContext.NewsAndTags on x.NewsId equals nt.NewsId 
                        where nt.NewsTagId == tagId 
                        select x;
            }

            if (categoryId > 0)
            {
                query = from x in query join nt in _dbContext.NewsAndCategories on x.NewsId equals nt.NewsId 
                        where nt.NewsCategoryId == categoryId
                        select x;
            }

            query.OrderByDescending(x => x.CreatedDate).ToList();
            List<NewsView> news = new List<NewsView>();
            foreach (var item in query)
            {
                if (item.IsIntroduction == true)
                {
                    continue;
                }
                var newsView = new NewsView()
                {
                    NewsId = item.NewsId,
                    Title = item.Title,
                    Image = item.Image,
                    Summary = item.Summary,
                    CustomDate = item.CreatedDate.Value.ToString("MMMM dd,yyyy")
                };
                news.Add(newsView);
            }

            ViewBag.name = name;
            ViewBag.tagId = tagId;
            ViewBag.categoryId = categoryId;
            ViewBag.searchName = name;

            PagedList<NewsView> lst = new PagedList<NewsView>(news, pageNumber, pageSize);
            return View(lst);
        }

        public IActionResult NewsDetail(int id)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (accClaim != null)
            {
                ViewBag.CurrentUsername = accClaim.Value;
                var accId = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().AccountId;
                var cartCount = _dbContext.Carts
                    .Where(x => x.AccountId == accId)
                    .ToList();
                ViewBag.CartCount = cartCount.Count();
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            var authorCurrentNewsId = _dbContext.News.Find(id).AccountId;

            var author = _dbContext.Accounts
                    .Where(x => x.AccountId == authorCurrentNewsId)
                    .FirstOrDefault();
            ViewBag.authorIntroduction = author.Introduction;
            ViewBag.authorImage = author.Image;

            var model = _dbContext.News.Find(id);

            var query = new NewsView()
            {
                NewsId = id,
                Title = model.Title,
                CustomDate = model.CreatedDate.Value.ToString("MMMM dd,yyyy"),
                Content = model.Content
            };

            return View(query);
        }

        public IActionResult GetAllNewsCategories()
        {
            var query = _dbContext.NewsCategories.ToList();
            return Json(query);
        }

        public IActionResult GetAllTags()
        {
            var query = _dbContext.NewsTags.ToList();
            return Json(query);
        }

        public IActionResult GetTags(int id)
        {
            var query = _dbContext.NewsTags
                .Join(_dbContext.NewsAndTags, a => a.NewsTagId, b => b.NewsTagId, (a, b) => new { a, b })
                .Where(x => x.b.NewsId == id)
                .ToList();
            List<NewsTag> tags = new List<NewsTag>();
            foreach (var item in query)
            {
                var tag = new NewsTag()
                {
                    Name = item.a.Name,
                    NewsTagId = item.a.NewsTagId
                };
                tags.Add(tag);
            }
            return Json(tags);
        }
        public IActionResult GetRecentNews()
        {
            var query = _dbContext.News.OrderBy(x => x.CreatedDate).Reverse().ToList();
            List<NewsView> news = new List<NewsView>();
            var cnt = 1;
            foreach(var item in query)
            {
                if (cnt == 3)
                {
                    break;
                }
                var model = new NewsView()
                {
                    NewsId = item.NewsId,
                    Title = item.Title,
                    Image = item.Image,
                    Summary = item.Summary,
                    CustomDate = item.CreatedDate.Value.ToString("MMMM dd,yyyy")
                };
                news.Add(model);
                cnt++;
            }
            return Json(news);
        }

    }
}
