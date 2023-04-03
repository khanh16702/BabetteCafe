using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/customer")]
    [Authorize(Roles = "Admin, Collaborator, Staff")]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment;
        public CustomerController(ILogger<CustomerController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [Route("Index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Customers
                .Where(x => string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.Trim().ToLower()));
            PagedList<Customer> lst = new PagedList<Customer>(query, pageNumber, pageSize);
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

        [Route("addorupdate")]
        public IActionResult AddOrUpdate(int id, int? page)
        {
            var customer = new Customer();
            if (id > 0)
            {
                var entity = _dbContext.Customers.Find(id);
                customer.CustomerId = entity.CustomerId;
                customer.Name = entity.Name;
                customer.Phone = entity.Phone;
                customer.CreatedDate = entity.CreatedDate;
                customer.UpdatedDate = entity.UpdatedDate;
                customer.CreatedBy = entity.CreatedBy;
                customer.UpdatedBy = entity.UpdatedBy;
            }
            ViewBag.page = page;
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var currentRoleName = _dbContext.Accounts
                .Join(_dbContext.Roles, a => a.RoleId, b => b.RoleId, (a, b) => new { a, b })
                .Where(x => x.a.Username == accClaim.Value)
                .FirstOrDefault()
                .b.Name;
            ViewBag.RoleName = currentRoleName;

            var thisAcc = _dbContext.Accounts.Where(x => x.Username == accClaim.Value).FirstOrDefault();
            ViewBag.User = accClaim.Value;
            ViewBag.AccAvatar = thisAcc.Image;
            ViewBag.accountId = thisAcc.AccountId;
            return View(customer);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(Customer model, int? page)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/customer/AddOrUpdate?id={model.CustomerId}&page={page}");
            }
            if (model.CustomerId > 0)
            {
                var query = _dbContext.Customers.Find(model.CustomerId);
                query.Name = model.Name;
                query.Phone = model.Phone;
                query.CreatedDate = model.CreatedDate;
                query.CreatedBy = model.CreatedBy;
                query.UpdatedBy = accClaim.Value;
                query.UpdatedDate = DateTime.Now;
                _dbContext.Customers.Update(query);
            }
            else
            {
                var customer = new Customer()
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    CreatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value,
                    UpdatedDate = DateTime.Now
                };
                _dbContext.Customers.Add(customer);
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/customer/index?page={page}");
        }

        [Route("ViewCart")]
        public IActionResult ViewCart(int customerId, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var query = _dbContext.Carts
                .Join(_dbContext.Products, a => a.ProductId, b => b.ProductId, (a,b) => new {a,b})
                .Where(x => x.a.CustomerId == customerId && x.a.IsPlaced == false)
                .ToList();
            List<CartView> listCV = new List<CartView>();
            foreach (var cart in query)
            {
                var cartView = new CartView()
                {
                    CartId = cart.a.CartId,
                    Name = cart.b.Name,
                    ProductPrice = cart.b.Price,
                    Quantity = cart.a.Quantity,
                    TotalPrice = cart.a.Quantity * cart.b.Price,
                    CustomerId = cart.a.CustomerId,
                };
                listCV.Add(cartView);
            }
            ViewBag.cartCount = listCV.Count();
            PagedList<CartView> lst = new PagedList<CartView>(listCV, pageNumber, pageSize);

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
            ViewBag.customerId = customerId;
            return View(lst);
        }

        [Route("GetAllProducts")]
        public IActionResult GetAllProducts(int categoryId)
        {
            List<Product> products = new List<Product>();
            if (categoryId > 0)
            {
                var prodcate = _dbContext.ProductAndCategories
                    .Join(_dbContext.Products, a => a.ProductId, b => b.ProductId, (a,b) => new {a,b})
                    .Where(x => x.a.ProductCategoryId == categoryId)
                    .ToList();
                foreach(var item in prodcate)
                {
                    var product = new Product()
                    {
                        ProductId = item.b.ProductId,
                        Name = item.b.Name,
                        Price = item.b.Price
                    };
                    products.Add(product);
                }
            }
            else
            {
                var productsQuery = _dbContext.Products.ToList();
                products = productsQuery.OrderBy(x => x.ProductId).ToList();
            }
            return Json(products);
        }

        [Route("addcart")]
        public IActionResult AddCart(int cusId, int? page)
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
            ViewBag.customerId = cusId;
            ViewBag.page = page;
            return View();
        }

        [Route("addcart")]
        [HttpPost]
        public IActionResult AddCart(Cart model, int? page) 
        {
            if (model.Quantity == 0 )
            {
                TempData["error"] = "You haven't chosen quantity!";
                return Redirect($"/admin/customer/addcart?cusId={model.CustomerId}&page={page}");
            }
            var cart = new Cart()
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                CustomerId = model.CustomerId,
                IsPlaced = false
            };

            var checkCartQuery = _dbContext.Carts
                .Where(x => x.CustomerId == model.CustomerId && x.ProductId == model.ProductId && x.IsPlaced == false)
                .FirstOrDefault();
            if (checkCartQuery != null)
            {
                checkCartQuery.Quantity += model.Quantity;
                var checkProductQuantity = _dbContext.Products.Find(checkCartQuery.ProductId);
                if (checkProductQuantity.Quantity < checkCartQuery.Quantity)
                {
                    TempData["error"] = $"{checkProductQuantity.Name} is not enough";
                    return Redirect($"/admin/customer/addcart?cusId={model.CustomerId}&page={page}");
                }
                else {
                    _dbContext.Carts.Update(checkCartQuery);
                    _dbContext.SaveChanges();
                    return Redirect($"/admin/customer/viewcart?customerId={model.CustomerId}&page={page}");
                }
            }
            else
            {
                var checkProductQuantity = _dbContext.Products.Find(model.ProductId);
                if (checkProductQuantity.Quantity < model.Quantity)
                {
                    TempData["error"] = $"{checkProductQuantity.Name} is not enough";
                    return Redirect($"/admin/customer/addcart?cusId={model.CustomerId}&page={page}");
                }
                else
                {
                    _dbContext.Carts.Add(cart);
                    _dbContext.SaveChanges();
                    return Redirect($"/admin/customer/viewcart?customerId={model.CustomerId}&page={page}");
                }
            }

        }

        [Route("Delete")]
        public IActionResult Delete(int id, int customerId, int? page)
        {
            var query = _dbContext.Carts.Find(id);
            _dbContext.Carts.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/admin/customer/viewcart?customerId={customerId}&page={page}");
        }

        [Route("createsalereceipt")]
        public IActionResult CreateSaleReceipt(int customerId, int staffId, int? page)
        {

            var salereceipt = new SalesReceipt()
            {
                CreatedDate = DateTime.Now,
                IsDelivered = true,
                ShippingFee = 0,
                AccountId = staffId
            };
            _dbContext.SalesReceipts.Add(salereceipt);
            _dbContext.SaveChanges();
            var query = _dbContext.Carts.Where(x => x.CustomerId == customerId && x.IsPlaced == false).ToList();
            var assignSaleReceipt = _dbContext.SalesReceipts.OrderByDescending(x => x.CreatedDate).FirstOrDefault().SalesReceiptId;
            foreach(var item in query)
            {
                item.SalesReceiptId = assignSaleReceipt;
                item.IsPlaced = true;
                _dbContext.Carts.Update(item);

                var product = _dbContext.Products.Find(item.ProductId);
                product.Quantity -= item.Quantity;
                _dbContext.Products.Update(product);
            }
            _dbContext.SaveChanges();
            return Redirect($"/admin/customer/viewcart?customerId={customerId}&page={page}");
        }
    }
}
