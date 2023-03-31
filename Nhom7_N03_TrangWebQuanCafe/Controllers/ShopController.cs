using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Drawing.Printing;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public ShopController(ILogger<ShopController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int? page, string name)
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

            int pageSize = 9;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Products;


            query.OrderByDescending(x => x.CreatedDate).ToList();
            PagedList<Product> lst = new PagedList<Product>(query, pageNumber, pageSize);
            return View(lst);
        }

        public IActionResult GetAllCategories()
        {
            var query = _dbContext.ProductCategories.ToList();
            List<CategoryView> categories = new List<CategoryView>();
            foreach (var item in query)
            {
                var category = new CategoryView()
                {
                    ProductCategoryId = item.ProductCategoryId,
                    Name = item.Name
                };

                var countProducts = _dbContext.ProductAndCategories
                    .Where(x => x.ProductCategoryId == item.ProductCategoryId)
                    .ToList()
                    .Count();
                category.NumberOfProducts = countProducts;
                categories.Add(category);
            }
            return Json(categories);
        }

        public IActionResult ShopDetail(int id)
        {
            var query = _dbContext.Products.Find(id);
            return View(query);
        }
    }
}
