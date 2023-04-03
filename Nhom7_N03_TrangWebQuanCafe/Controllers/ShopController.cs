using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
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

        public IActionResult Index(int? page, string name, int categoryId, int sortId, float minPrice = 15, float maxPrice = 100)
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
                var accRole = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().RoleId;
                ViewBag.accRole = accRole;
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            int pageSize = 9;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Products
                .Where(x => x.Price >= minPrice && x.Price <= maxPrice)
                .AsQueryable();

            if (name != null)
            {
                query = query.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            }
            if (categoryId > 0)
            {
                query = from x in query
                        join pc in _dbContext.ProductAndCategories on x.ProductId equals pc.ProductId
                        where pc.ProductCategoryId == categoryId
                        select x;
            }

            switch (sortId)
            {
                case 1:
                    query = from x in query orderby x.CreatedDate descending select x;
                    break;
                case 2:
                    query = from x in query orderby x.Price select x;
                    break;
                case 3:
                    query = from x in query orderby x.Price descending select x;
                    break;
                case 4:
                    query = from x in query orderby x.Name select x;
                    break;
                default:
                    query = from x in query orderby x.CreatedDate descending select x;
                    break;
            }

            ViewBag.name = name;
            ViewBag.categoryId = categoryId;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            ViewBag.sortId = sortId;

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

        public IActionResult GetRecentProducts()
        {
            var query = _dbContext.Products.OrderByDescending(x => x.CreatedDate).Take(2).ToList();
            return Json(query);
        }

        public IActionResult ShopDetail(int id)
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
                var accRole = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().RoleId;
                ViewBag.accRole = accRole;
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            var query = _dbContext.Products.Find(id);
            ViewBag.quantity = query.Quantity;

            return View(query);
        }

        public IActionResult GetProductCategories(int id)
        {
            var query = _dbContext.ProductAndCategories
                .Join(_dbContext.ProductCategories, a => a.ProductCategoryId, b => b.ProductCategoryId, (a,b) => new {a,b})
                .Where(x => x.a.ProductId == id)
                .ToList();
            List<ProductCategory> lst = new List<ProductCategory>();
            foreach (var item in query)
            {
                var category = new ProductCategory()
                {
                    ProductCategoryId = item.b.ProductCategoryId,
                    Name = item.b.Name
                };
                lst.Add(category);
            }
            return Json(lst);
        }

        public IActionResult GetRelatedProducts(int id)
        {
            var query = _dbContext.ProductAndCategories
                .Where(x => x.ProductId == id)
                .ToList();
            List<Product> lst = new List<Product>();
            var count = 1;
            foreach(var item in query)
            {
                if (count == 4)
                {
                    break;
                }
                
                var listProducts = _dbContext.ProductAndCategories
                    .Join(_dbContext.Products, a => a.ProductId, b => b.ProductId, (a,b) => new {a,b})
                    .Where(x => x.a.ProductCategoryId == item.ProductCategoryId && x.a.ProductId != id)
                    .ToList();
                foreach(var model in listProducts)
                {
                    if (count == 4)
                    {
                        break;
                    }
                    var product = new Product()
                    {
                        ProductId = model.b.ProductId,
                        Name = model.b.Name,
                        Image = model.b.Image,
                        Price = model.b.Price
                    };
                    lst.Add(product);
                    count++;
                }
            }
            return Json(lst);
        }

        public IActionResult AddToCart(int id, int buyQuantity)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var accId = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().AccountId;

            var queryCheckAccountCart = _dbContext.Carts
                .Where(x => x.AccountId == accId && x.ProductId == id && x.IsPlaced == false)
                .FirstOrDefault();
            if (queryCheckAccountCart != null)
            {
                queryCheckAccountCart.Quantity += buyQuantity;
                _dbContext.Update(queryCheckAccountCart);
            }
            else
            {
                var queryCheckSoldOut = _dbContext.Products.Find(id);
                if (queryCheckSoldOut.Quantity == 0)
                {
                    return Json(new { result = "soldout" });
                }
                var queryCheckCartHasItem = _dbContext.Carts
                    .Where(x => x.AccountId == accId && x.IsPlaced == false)
                    .FirstOrDefault();
                var cartModel = new Cart()
                {
                    Quantity = buyQuantity,
                    ProductId = id,
                    AccountId = accId,
                    IsPlaced = false
                };
                if (queryCheckCartHasItem != null)
                {
                    cartModel.SalesReceiptId = queryCheckCartHasItem.SalesReceiptId;
                }
                else
                {
                    var saleReceipt = new SalesReceipt()
                    {
                        CreatedDate = DateTime.Now
                    };
                    _dbContext.SalesReceipts.Add(saleReceipt);
                    _dbContext.SaveChanges();
                    cartModel.SalesReceiptId = _dbContext.SalesReceipts.OrderByDescending(x => x.CreatedDate).FirstOrDefault().SalesReceiptId;
                }
                _dbContext.Carts.Add(cartModel);
            }
            _dbContext.SaveChanges();
            return Json(new { result = "success" });

        }
    }
}
