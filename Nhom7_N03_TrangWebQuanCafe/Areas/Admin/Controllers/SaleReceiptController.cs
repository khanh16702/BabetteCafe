using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using System.Xml.Linq;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/SaleReceipt")]
    [Authorize(Roles = "Collaborator, Admin, Staff")]
    public class SaleReceiptController : Controller
    {
        private readonly ILogger<SaleReceiptController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public SaleReceiptController(ILogger<SaleReceiptController> logger)
        {
            _logger = logger;
        }

        [Route("index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.SalesReceipts
                .Where(x => x.IsDelivered != null)
                .Select(x => new SalesReceipt()
                {
                    SalesReceiptId = x.SalesReceiptId,
                    CreatedDate = x.CreatedDate,
                    IsDelivered = x.IsDelivered,
                    ShippingFee = x.ShippingFee,
                    StaffId = x.StaffId
                })
                .ToList();
            PagedList<SalesReceipt> lst = new PagedList<SalesReceipt>(query, pageNumber, pageSize);
            ViewBag.page = page;

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
            return View(lst);
        }

        [Route("GetDetail")]
        public IActionResult GetDetail(int id, int? page)
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

            var query = _dbContext.Carts
                .Join(_dbContext.Products, a => a.ProductId, b => b.ProductId, (a,b) => new {a,b})
                .Where(x => x.a.SalesReceiptId == id)
                .ToList();
            List<CartView> lst = new List<CartView>();
            foreach(var item in query)
            {
                var cart = new CartView()
                {
                    CartId = item.a.CartId,
                    Name = item.b.Name,
                    ProductPrice = item.b.Price,
                    Quantity = item.a.Quantity,
                    TotalPrice = item.a.Quantity * item.b.Price
                };
                lst.Add(cart);
            }
            ViewBag.page = page;
            return View(lst);
        }

        [Route("UpdateDelivered")]
        public IActionResult UpdateDelivered(int id, bool isDelivered, int? page)
        {
            var query = _dbContext.SalesReceipts.Find(id);
            query.IsDelivered = isDelivered == true ? false : true;
            _dbContext.SalesReceipts.Update(query);
            _dbContext.SaveChanges();
            ViewBag.page = page;
            return Redirect($"/admin/SaleReceipt/index?page={page}");
        }
    }
}
