using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.productQuantityError = TempData["productQuantityError"];
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            ViewBag.CurrentUsername = accClaim.Value;
            var account = _dbContext.Accounts
                .Where(x => x.Username == accClaim.Value)
                .FirstOrDefault();
            var accId = account.AccountId;
            ViewBag.accId = accId;
            var cartCount = _dbContext.Carts
                .Where(x => x.AccountId == accId && x.IsPlaced == false)
                .ToList();
            ViewBag.CartCount = cartCount.Count();
            return View(account);
        }

        public IActionResult GetCart(int accId, string productName, string act)
        {
            var query = _dbContext.Carts
                .Where(x => x.AccountId == accId && x.IsPlaced == false)
                .ToList();
            if (productName == "")
            {
                List<CartView> products = new List<CartView>();
                foreach (var item in query)
                {
                    var product = new CartView()
                    {
                        Quantity = item.Quantity
                    };
                    var productQuery = _dbContext.Products.Find(item.ProductId);
                    product.Name = productQuery.Name;
                    product.Image = productQuery.Image;
                    product.ProductPrice = productQuery.Price;
                    product.TotalPrice = product.Quantity * productQuery.Price;
                    products.Add(product);
                }
                return Json(products);
            }
            else
            {
                List<CartView> products = new List<CartView>();
                foreach (var item in query)
                {
                    var product = new CartView();
                    var productQuery = _dbContext.Products
                        .Where(x => x.ProductId == item.ProductId && x.Name == productName)
                        .FirstOrDefault();
                    if (productQuery != null)
                    {
                        if (act == "remove")
                        {
                            _dbContext.Carts.Remove(item);
                            _dbContext.SaveChanges();
                            continue;
                        }
                        if (act == "inc")
                        {
                            item.Quantity++;
                            _dbContext.Carts.Update(item);
                        }
                        if (act == "dec")
                        {
                            item.Quantity--;
                            if (item.Quantity < 1)
                            {
                                item.Quantity = 1;
                            }
                            _dbContext.Carts.Update(item);
                        }

                        product.Quantity = item.Quantity;
                        product.Name = productQuery.Name;
                        product.Image = productQuery.Image;
                        product.ProductPrice = productQuery.Price;
                        product.TotalPrice = product.Quantity * product.ProductPrice;

                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        product.Quantity = item.Quantity;
                        var _productQuery = _dbContext.Products.Find(item.ProductId);
                        product.Name = _productQuery.Name;
                        product.Image = _productQuery.Image;
                        product.ProductPrice = _productQuery.Price;
                        product.TotalPrice = product.Quantity * product.ProductPrice;
                    }
                    products.Add(product);
                }
                return Json(products);
            }
        }

        public IActionResult GetCartTotal(int accId)
        {
            var query = _dbContext.Carts.Where(x => x.AccountId == accId && x.IsPlaced == false).ToList();
            double? total = 0.0;
            foreach (var item in query)
            {
                var price = _dbContext.Products.Find(item.ProductId).Price;
                total += item.Quantity * price;
            }
            var subtotal = total;
            var accountQuery = _dbContext.Accounts.Where(x => x.AccountId == accId).FirstOrDefault();
            var discountCodeQuery = _dbContext.SaleReceiptAndDiscounts
                .Join(_dbContext.SalesReceipts, a => a.SalesReceiptId, b => b.SalesReceiptId, (a, b) => new { a, b })
                .Join(_dbContext.Carts, x => x.b.SalesReceiptId, c => c.SalesReceiptId, (x, c) => new { x, c })
                .Join(_dbContext.Accounts, y => y.c.AccountId, d => d.AccountId, (y, d) => new { y, d })
                .Where(z => z.d.AccountId == accId && z.y.c.IsPlaced == false)
                .ToList();
            List<int?> writeDiscountCode = new List<int?>();
            foreach(var item in discountCodeQuery)
            {
                var isContinue = false;
                foreach(var i in writeDiscountCode)
                {
                    if (i == item.y.x.a.DiscountCodeId)
                    {
                        isContinue = true;
                        break;
                    }
                }
                if (isContinue)
                {
                    continue;
                }
                else
                {
                    writeDiscountCode.Add(item.y.x.a.DiscountCodeId);
                }
                var dcQuery = _dbContext.DiscountCodes.Where(x => x.DiscountCodeId == item.y.x.a.DiscountCodeId).FirstOrDefault();
                total = (total - dcQuery.MinimumToApply) > 0 ? total - dcQuery.MinimumToApply : 0;
            }
            return Json(new { objSubTotal = subtotal, objTotal = total });
        }

        public IActionResult ApplyCoupon(string discountName, int accId)
        {
            var cartQuery = _dbContext.Carts.Where(x => x.IsPlaced == false && x.AccountId == accId).ToList();
            if (cartQuery.Count() == 0)
            {
                return Json(new { status = "no products" });
            }

            var findDiscount = _dbContext.DiscountCodes
                .Where(x => x.Name == discountName && DateTime.Compare(x.ActiveDate.Value, DateTime.Now) <= 0
                && DateTime.Compare(x.ExpireDate.Value, DateTime.Now) > 0)
                .FirstOrDefault();
            if (findDiscount == null)
            {
                return Json(new { status = "error" });
            }

            var productsInCart = _dbContext.Carts
                .Join(_dbContext.Products, a => a.ProductId, b => b.ProductId, (a, b) => new { a, b })
                .Where(x => x.a.IsPlaced == false && x.a.AccountId == accId)
                .ToList();
            double? total = 0.0;
            foreach (var product in productsInCart)
            {
                total += product.a.Quantity * product.b.Price;
            }

            var usedDiscountCode = _dbContext.DiscountCodes
                .Join(_dbContext.SaleReceiptAndDiscounts, a => a.DiscountCodeId, b => b.DiscountCodeId, (a, b) => new { a, b })
                .Join(_dbContext.SalesReceipts, x => x.b.SalesReceiptId, c => c.SalesReceiptId, (x, c) => new { x, c })
                .Join(_dbContext.Carts, y => y.c.SalesReceiptId, d => d.SalesReceiptId, (y, d) => new { y, d })
                .Join(_dbContext.Accounts, z => z.d.AccountId, e => e.AccountId, (z, e) => new { z, e })
                .Where(w => w.e.AccountId == accId && w.z.d.IsPlaced == true)
                .ToList();
            foreach(var item in usedDiscountCode)
            {
                if (item.z.y.x.a.Name == discountName)
                {
                    return Json(new { status = "discount used" });
                }
            }

            var typedDiscountCode = _dbContext.DiscountCodes
                .Join(_dbContext.SaleReceiptAndDiscounts, a => a.DiscountCodeId, b => b.DiscountCodeId, (a, b) => new { a, b })
                .Join(_dbContext.SalesReceipts, x => x.b.SalesReceiptId, c => c.SalesReceiptId, (x, c) => new { x, c })
                .Join(_dbContext.Carts, y => y.c.SalesReceiptId, d => d.SalesReceiptId, (y, d) => new { y, d })
                .Join(_dbContext.Accounts, z => z.d.AccountId, e => e.AccountId, (z, e) => new { z, e })
                .Where(w => w.e.AccountId == accId && w.z.d.IsPlaced == false)
                .ToList();
            foreach (var item in typedDiscountCode)
            {
                total -= item.z.y.x.a.DecreaseAmount;
                if (item.z.y.x.a.Name == discountName)
                {
                    return Json(new { status = "discount typed" });
                }
            }

            if (findDiscount.MinimumToApply > total)
            {
                return Json(new { status = "not enough value" });
            }

            var getSalesReceiptId = _dbContext.Carts.Where(x => x.IsPlaced == false && x.AccountId == accId).FirstOrDefault().SalesReceiptId;
            var getDiscountCodeId = findDiscount.DiscountCodeId;
            var sql = $"insert into SaleReceiptAndDiscount values({getSalesReceiptId}, {getDiscountCodeId})";
            _dbContext.Database.ExecuteSqlRaw(sql);
            _dbContext.SaveChanges();
            return Json(new {status = "success"});
        }

        public IActionResult ClearTypedDiscount(int accId)
        {
            var getSalesReceiptId = _dbContext.Carts.Where(x => x.IsPlaced == false && x.AccountId == accId).FirstOrDefault().SalesReceiptId;
            var sql = $"delete from SaleReceiptAndDiscount where SalesReceiptId = {getSalesReceiptId}";
            _dbContext.Database.ExecuteSqlRaw(sql);
            _dbContext.SaveChanges();
            return Json(new { status = "success" });
        }

        public IActionResult TypedDiscountCount(int accId)
        {
            var discountCodeQuery = _dbContext.SaleReceiptAndDiscounts
                .Join(_dbContext.SalesReceipts, a => a.SalesReceiptId, b => b.SalesReceiptId, (a, b) => new { a, b })
                .Join(_dbContext.Carts, x => x.b.SalesReceiptId, c => c.SalesReceiptId, (x, c) => new { x, c })
                .Join(_dbContext.Accounts, y => y.c.AccountId, d => d.AccountId, (y, d) => new { y, d })
                .Where(z => z.d.AccountId == accId && z.y.c.IsPlaced == false)
                .ToList();
            List<int?> discountCodes = new List<int?>();
            foreach(var item in discountCodeQuery)
            {
                var isContinue = false;
                foreach(var discountCode in discountCodes)
                {
                    if (discountCode == item.y.x.a.DiscountCodeId)
                    {
                        isContinue = true;
                        break;
                    }
                }
                if (isContinue)
                {
                    continue;
                }
                discountCodes.Add(item.y.x.a.DiscountCodeId);

            }
            return Json(discountCodes.Count());
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
