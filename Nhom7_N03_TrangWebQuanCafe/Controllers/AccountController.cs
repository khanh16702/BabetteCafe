using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment; 
        public AccountController(ILogger<AccountController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (accClaim == null)
            {
                return Redirect("/login/index");
            }

            var acc = _dbContext.Accounts.Where(x => x.Username == accClaim.Value).FirstOrDefault();

            ViewBag.avatar = acc.Image;
            ViewBag.username = acc.Username;
            ViewBag.password = acc.Password;

            ViewBag.CurrentUsername = accClaim.Value;
            var query = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value);
            var cartCount = _dbContext.Carts
                .Join(_dbContext.Accounts, a => a.AccountId, b => b.AccountId, (a,b) => new {a,b} )
                .Where(x => x.b.Username == accClaim.Value && x.a.IsPlaced == false)
                .ToList();
            ViewBag.CartCount = cartCount.Count();

            var roleId = acc.RoleId;
            ViewBag.firstName = acc.FirstName;
            ViewBag.lastName = acc.LastName;
            ViewBag.address = acc.Address;
            ViewBag.email = acc.Email;
            ViewBag.phone = acc.Phone;
            ViewBag.introduction = acc.Introduction;
            ViewBag.displayName = acc.DisplayName;
            ViewBag.backgroundColor = "black";

            switch (roleId)
            {
                case 1:
                    ViewBag.Role = "admin";
                    break;
                case 2:
                    ViewBag.Role = "staff";
                    break;
                case 3:
                    ViewBag.Role = "collaborator";
                    break;
                case 5:
                    ViewBag.Role = "customer";
                    break;
                case 6:
                    ViewBag.Role = "user account";
                    break;
                default:
                    break;
            }
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Json(new { status = "error" });
            }
            string folderUploads = Path.Combine(_environment.WebRootPath, "assets\\img\\account-image");
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folderUploads, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string filePath = "/assets/img/account-image/" + fileName;
            return Json(new
            {
                status = "success",
                filePath
            });
        }

        [HttpPost]
        public IActionResult UpdateAccount(AccountView model, int placeOrder)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            var myAcc = _dbContext.Accounts
                .Where(x => x.Username == accClaim.Value)
                .FirstOrDefault();
            if (model.Password != null)
            {
                if (model.Password != model.RetypedPassword)
                {
                    TempData["error"] = "Confirm password failed";
                    return Redirect("/account/index");
                }
                string pass = model.Password;
                myAcc.Password = pass;
            }

            myAcc.Email = model.Email;
            myAcc.DisplayName = model.DisplayName;
            myAcc.FirstName = model.FirstName;
            myAcc.LastName = model.LastName;
            myAcc.Address = model.Address;
            myAcc.Phone = model.Phone;
            myAcc.Introduction = model.Introduction;
            myAcc.Image = model.Image;

            _dbContext.Accounts.Update(myAcc);
            _dbContext.SaveChanges();
            if (placeOrder > 0)
            {
                var queryBuyDone = _dbContext.Carts
                    .Where(x => x.AccountId == myAcc.AccountId && x.IsPlaced == false)
                    .ToList();
                foreach(var item in queryBuyDone)
                {
                    item.IsPlaced = true;
                    var queryUpdateProduct = _dbContext.Products.Find(item.ProductId);
                    if (queryUpdateProduct.Quantity >= item.Quantity)
                    {
                        queryUpdateProduct.Quantity -= item.Quantity;
                    }
                    else
                    {
                        TempData["productQuantityError"] = $"{queryUpdateProduct.Name} is not enough in stock right now :(";
                        return Redirect("/cart/index");
                    }
                    _dbContext.Products.Update(queryUpdateProduct);
                    _dbContext.Carts.Update(item);
                }
                var saleReceipt = _dbContext.SalesReceipts.Find(queryBuyDone.FirstOrDefault().SalesReceiptId);
                saleReceipt.CreatedDate = DateTime.Now;
                saleReceipt.IsDelivered = false;
                saleReceipt.ShippingFee = 0;
                _dbContext.SalesReceipts.Update(saleReceipt);

                _dbContext.SaveChanges();
                return Redirect("/cart/thankyou");
            }
            return Redirect("/account/index");
        }
    }
}
