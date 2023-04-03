using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public GalleryController(ILogger<GalleryController> logger)
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

            var query = _dbContext.Galleries.ToList();
            return View(query);
        }
    }
}
