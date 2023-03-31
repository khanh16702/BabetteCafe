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
    [Route("admin/newscategory")]
    [Authorize(Roles = "Collaborator, Admin")]
    public class NewsCategoryController : Controller
    {
        private readonly ILogger<NewsCategoryController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public NewsCategoryController(ILogger<NewsCategoryController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.NewsCategories.Where(x => string.IsNullOrEmpty(name) ||
                                                     x.Name.ToLower().Contains(name.Trim().ToLower()));
            ViewBag.page = page;
            PagedList<NewsCategory> lst = new PagedList<NewsCategory>(query, pageNumber, pageSize);

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
            var viewCategory = new NewsCategory();
            if (id > 0)
            {
                var entity = _dbContext.NewsCategories.Find(id);
                viewCategory = new NewsCategory()
                {
                    NewsCategoryId = entity.NewsCategoryId,
                    Name = entity.Name,
                    NumberOfPosts = entity.NumberOfPosts,
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
        public IActionResult AddOrUpdate(NewsCategory pcat, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/newscategory/AddOrUpdate?id={pcat.NewsCategoryId}&page={page}");
            }
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (pcat.NewsCategoryId == 0)
            {
                var checkQuery = _dbContext.NewsCategories.Where(x => x.Name == pcat.Name).FirstOrDefault();
                if (checkQuery != null)
                {
                    TempData["error"] = "This category has been in database already!";
                    return Redirect($"/Admin/newscategory/AddOrUpdate?id={pcat.NewsCategoryId}&page={page}");
                }
                var entity = new NewsCategory()
                {
                    Name = pcat.Name,
                    NumberOfPosts = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value
                };
                _dbContext.NewsCategories.Add(entity);
            }
            else
            {
                var thisNCat = _dbContext.NewsCategories.Find(pcat.NewsCategoryId);
                thisNCat.Name = pcat.Name;
                thisNCat.NumberOfPosts = pcat.NumberOfPosts;
                thisNCat.UpdatedDate = DateTime.Now;
                thisNCat.CreatedBy = pcat.CreatedBy;
                thisNCat.CreatedDate = pcat.CreatedDate;
                thisNCat.UpdatedBy = accClaim.Value;
                _dbContext.NewsCategories.Update(thisNCat);
            }
            _dbContext.SaveChanges();
            return Redirect($"/admin/newscategory/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var sql = $"delete from NewsAndCategory where NewsCategoryId = {id}";
            _dbContext.Database.ExecuteSqlRaw(sql);
            var query = _dbContext.NewsCategories.Find(id);
            _dbContext.NewsCategories.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/admin/newscategory/index?page={page}");
        }
    }
}
