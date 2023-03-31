using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query;
using Nhom7_N03_TrangWebQuanCafe.Classes;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/account")]
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
        [Route("Index")]
        [Authorize(Roles = "Admin, Collaborator")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Accounts
                .Join(_dbContext.Roles, a => a.RoleId, b => b.RoleId, (a, b) => new { a, b })
                .Where(x => string.IsNullOrEmpty(name) || x.a.DisplayName.ToLower().Contains(name.Trim().ToLower()))
                .Select(x => new AccountView()
                {
                    AccountId = x.a.AccountId,
                    Username = x.a.Username,
                    Password = x.a.Password,
                    DisplayName = x.a.DisplayName,
                    Phone = x.a.Phone,
                    Email = x.a.Email,
                    Image = x.a.Image,
                    RoleName = x.b.Name
                })
                .ToList();

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
            ViewBag.page = page;

            List<AccountView> queryAccounts = new List<AccountView>();

            if (currentRoleName != "Admin")
            {
                foreach(var item in query)
                {
                    if (item.RoleName != "Admin")
                    {
                        queryAccounts.Add(item);
                    }
                }
            }
            else
            {
                queryAccounts = query;
            }

            PagedList<AccountView> lst = new PagedList<AccountView>(queryAccounts, pageNumber, pageSize);
            return View(lst);
        }

        [Route("AddOrUpdate")]
        public IActionResult AddOrUpdate(int id, int? page)
        {
            var account = new AccountView();
            if (id > 0)
            {
                var entity = _dbContext.Accounts.Find(id);
                account.AccountId = entity.AccountId;
                account.RoleId = entity.RoleId;
                account.Username = entity.Username;
                if (entity.Username != "admin")
                {
                    account.Password = entity.Password;
                }
                else
                {
                    account.Password = entity.Password;
                }
                account.DisplayName = entity.DisplayName;
                account.Phone = entity.Phone;
                account.Email = entity.Email;
                account.Image = entity.Image;
                account.LastName = entity.LastName;
                account.FirstName = entity.FirstName;
                account.Address = entity.Address;
                account.Introduction = entity.Introduction;

                var getRoleName = _dbContext.Roles.Find(entity.RoleId);
                account.RoleName = getRoleName.Name;
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
            return View(account);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(AccountView model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/account/AddOrUpdate?id={model.AccountId}&page={page}");
            }
            if (model.AccountId > 0)
            {
                var query = _dbContext.Accounts.Find(model.AccountId);
                query.Username = model.Username;
                query.DisplayName = model.DisplayName;
                query.Password = model.Password;
                query.Introduction = model.Introduction;
                query.Email = model.Email;
                query.Image = model.Image;
                query.Phone = model.Phone;
                query.RoleId = model.RoleId;
                _dbContext.Accounts.Update(query);
            }
            else
            {
                var queryCheck = _dbContext.Accounts.Where(x => x.Username == model.Username);
                if (queryCheck.Count() > 0)
                {
                    TempData["Error"] = "Username already exists";
                    return Redirect($"/Admin/account/AddOrUpdate?id={model.AccountId}&page={page}");
                }
                var account = new Account()
                {
                    Username = model.Username,
                    DisplayName = model.DisplayName,
                    Password = model.Password,
                    Introduction = model.Introduction,
                    Email = model.Email,
                    Image = model.Image,
                    Phone = model.Phone,
                    RoleId = model.RoleId
                };
                _dbContext.Accounts.Add(account);
            }
            _dbContext.SaveChanges();

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var currentAccountId = _dbContext.Accounts.Where(x => x.Username == accClaim.Value).FirstOrDefault().AccountId;

            if (currentAccountId == model.AccountId)
                return Redirect($"/admin/account/addorupdate?id={currentAccountId}");
            else 
            {
                if (currentAccountId != 6)
                {
                    return Redirect($"/admin/account/index?page={page}");
                }
                else
                {
                    return Redirect($"/account/index");
                }
            }
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var deleteGallery = _dbContext.Galleries
                .Where(x => x.AccountId == id);
            foreach (var item in deleteGallery)
            {
                _dbContext.Galleries.RemoveRange(deleteGallery);
            }

            var deleteCart = _dbContext.Carts
                .Where(x => x.AccountId == id);
            foreach (var item in deleteCart)
            {
                _dbContext.Carts.RemoveRange(deleteCart);
            }

            var deleteTable = _dbContext.TableShopAndAccounts
                .Where(x => x.AccountId == id);
            foreach (var item in deleteTable)
            {
                _dbContext.TableShopAndAccounts.RemoveRange(deleteTable);
            }

            var deleteNews = _dbContext.News
                .Where(x => x.AccountId == id)
                .ToList();
            foreach (var news in deleteNews)
            {
                var deleteNewsAndTags = _dbContext.NewsAndTags
                    .Where(x => x.NewsId == news.NewsId);
                _dbContext.NewsAndTags.RemoveRange(deleteNewsAndTags);

                var deleteNewsAndCategories = _dbContext.NewsAndCategories
                    .Where(x => x.NewsId == news.NewsId);
                _dbContext.NewsAndCategories.RemoveRange(deleteNewsAndCategories);
            }
            _dbContext.News.RemoveRange(deleteNews);

            var deleteAccounts = _dbContext.Accounts.Where(x => x.AccountId == id);
            _dbContext.Accounts.RemoveRange(deleteAccounts);

            _dbContext.SaveChanges();
            return Redirect($"/Admin/account/index?page={page}");
        }

        [Route("getallroles")]
        public IActionResult GetAllRoles()
        {
            var query = _dbContext.Roles.ToList();
            return Json(query);
        }

        [Route("uploadfile")]
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
    }
}
