using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using System.Xml.Linq;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/category")]
    [Authorize(Roles = "Collaborator, Admin")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.ProductCategories.Where(x => string.IsNullOrEmpty(name) ||
                                                     x.Name.ToLower().Contains(name.Trim().ToLower()));
            ViewBag.page = page;
            PagedList<ProductCategory> lst = new PagedList<ProductCategory>(query, pageNumber, pageSize);

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
            var viewCategory = new ProductCategory();
            if (id > 0)
            {
                var entity = _dbContext.ProductCategories.Find(id);
                viewCategory = new ProductCategory()
                {
                    ProductCategoryId = entity.ProductCategoryId,
                    Name = entity.Name,
                    CreatedBy = entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    UpdatedBy = entity.UpdatedBy,
                    UpdatedDate = entity.UpdatedDate
                };
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
            return View(viewCategory);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(ProductCategory pcat, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/Category/AddOrUpdate?id={pcat.ProductCategoryId}&page={page}");
            }
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (pcat.ProductCategoryId == 0)
            {
                var checkQuery = _dbContext.ProductCategories.Where(x => x.Name == pcat.Name).FirstOrDefault();
                if (checkQuery != null)
                {
                    TempData["error"] = "This category has been in database already!";
                    return Redirect($"/Admin/Category/AddOrUpdate?id={pcat.ProductCategoryId}&page={page}");
                }
                var entity = new ProductCategory()
                {
                    Name = pcat.Name,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value
                };
                _dbContext.ProductCategories.Add(entity);
            }
            else
            {
                var thisPCat = _dbContext.ProductCategories.Find(pcat.ProductCategoryId);
                thisPCat.Name = pcat.Name;
                thisPCat.UpdatedDate = DateTime.Now;
                thisPCat.CreatedBy = pcat.CreatedBy;
                thisPCat.CreatedDate = pcat.CreatedDate;
                thisPCat.UpdatedBy = accClaim.Value;
                _dbContext.ProductCategories.Update(thisPCat);
            }
            _dbContext.SaveChanges();
            return Redirect($"/admin/category/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var sql = $"delete from ProductAndCategory where ProductCategoryId = {id}";
            _dbContext.Database.ExecuteSqlRaw(sql);
            var query = _dbContext.ProductCategories.Find(id);
            _dbContext.ProductCategories.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/admin/category/index?page={page}");
        }
    }
}
