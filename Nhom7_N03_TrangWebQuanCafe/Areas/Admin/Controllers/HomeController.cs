using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/home")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index()
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            ViewBag.User = accClaim.Value;

            var thisAcc = _dbContext.Accounts
                .Where(x => x.Username == accClaim.Value)
                .FirstOrDefault();
            ViewBag.AccAvatar = thisAcc.Image;
            ViewBag.accountId = thisAcc.AccountId;
            var currentRoleName = _dbContext.Accounts
                .Join(_dbContext.Roles, a => a.RoleId, b => b.RoleId, (a, b) => new { a, b })
                .Where(x => x.a.Username == accClaim.Value)
                .FirstOrDefault()
                .b.Name;
            ViewBag.RoleName = currentRoleName;

            var countRecentOrders = _dbContext.SalesReceipts
                .Where(x => x.IsDelivered != null && DateTime.Compare(x.CreatedDate.Value, DateTime.Now.AddDays(-14)) >= 0
                && DateTime.Compare(x.CreatedDate.Value, DateTime.Now) <= 0)
                .ToList()
                .Count();
            ViewBag.NewOrders = countRecentOrders;
            return View();
        }

        [Route("GetSaleNumber")]
        public IActionResult GetSaleNumber()
        {
            int? sellQuantity = 0;
            var query = _dbContext.SalesReceipts
                .Join(_dbContext.Carts, a => a.SalesReceiptId, b => b.SalesReceiptId, (a, b) => new { a, b })
                .Where(x => x.a.IsDelivered == true)
                .ToList();
            foreach (var item in query)
            {
                sellQuantity += item.b.Quantity;
            }
            int? productInStock = 0;
            var products = _dbContext.Products.ToList();
            foreach(var item in products)
            {
                productInStock += item.Quantity;
            }
            return Json(new { sellQuantity, productInStock });
        }

        [Route("GetRevenueOfMonth")]
        public IActionResult GetRevenueOfMonth()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            double? revenue = 0.0;
            var query = _dbContext.SalesReceipts
                .Join(_dbContext.Carts, a => a.SalesReceiptId, b => b.SalesReceiptId, (a,b) => new {a,b})
                .Join(_dbContext.Products, x => x.b.ProductId, c => c.ProductId, (x,c) => new {x,c})
                .Where(y => y.x.a.IsDelivered == true && DateTime.Compare(y.x.a.CreatedDate.Value, startDate) >= 0
                && DateTime.Compare(y.x.a.CreatedDate.Value, endDate) <= 0)
                .ToList();
            foreach(var item in query)
            {
                revenue += item.c.Price * item.x.b.Quantity;
            }

            return Json(revenue);
        }
    }
}
