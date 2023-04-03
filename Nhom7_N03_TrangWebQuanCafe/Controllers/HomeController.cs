using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext(); 
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
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
                    .Where(x => x.AccountId == accId && x.IsPlaced == false)
                    .ToList();
                ViewBag.CartCount = cartCount.Count();
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            var query = _dbContext.Banners.ToList();
            return View(query);
        }

        public IActionResult GetAllBanners()
        {
            var query = _dbContext.Banners.ToList();
            return Json(query);
        }

        public IActionResult GetIntroducingPost()
        {
            var query = _dbContext.News.Where(x => x.IsIntroduction == true).ToList();
            return Json(query);
        }

        public IActionResult Get8Gallery()
        {
            var query = _dbContext.Galleries.ToList();
            List<Gallery> galleries = new List<Gallery>();
            var cnt = 1;
            foreach(var item in query)
            {
                if (cnt == 9)
                {
                    break;
                }
                galleries.Add(item);
                cnt++;
            }
            return Json(galleries);
        }

        public IActionResult Get3News()
        {
            var query = _dbContext.News.Where(x => x.IsIntroduction == false).OrderBy(x => x.CreatedDate).Reverse().ToList();
            List<NewsView> news = new List<NewsView>();
            var cnt = 1;
            foreach (var item in query)
            {
                if (cnt == 4)
                {
                    break;
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
                cnt++;
            }
            return Json(news);
        }
    }
}