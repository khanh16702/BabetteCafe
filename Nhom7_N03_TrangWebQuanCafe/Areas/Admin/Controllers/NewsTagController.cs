using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("admin/newstag")]
    [Authorize(Roles = "Collaborator, Admin")]
    public class NewsTagController : Controller
    {
        private readonly ILogger<NewsTagController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public NewsTagController(ILogger<NewsTagController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.NewsTags.Where(x => string.IsNullOrEmpty(name) ||
                                                     x.Name.ToLower().Contains(name.Trim().ToLower()));
            ViewBag.page = page;
            PagedList<NewsTag> lst = new PagedList<NewsTag>(query, pageNumber, pageSize);

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
            var viewTag = new NewsTag();
            if (id > 0)
            {
                var entity = _dbContext.NewsTags.Find(id);
                viewTag = new NewsTag()
                {
                    NewsTagId = entity.NewsTagId,
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
            return View(viewTag);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(NewsTag pcat, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/newstag/AddOrUpdate?id={pcat.NewsTagId}&page={page}");
            }
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (pcat.NewsTagId == 0)
            {
                var checkQuery = _dbContext.NewsTags.Where(x => x.Name == pcat.Name).FirstOrDefault();
                if (checkQuery != null)
                {
                    TempData["error"] = "This category has been in database already!";
                    return Redirect($"/Admin/newstag/AddOrUpdate?id={pcat.NewsTagId}&page={page}");
                }
                var entity = new NewsTag()
                {
                    Name = pcat.Name,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value
                };
                _dbContext.NewsTags.Add(entity);
            }
            else
            {
                var thisNTag = _dbContext.NewsTags.Find(pcat.NewsTagId);
                thisNTag.Name = pcat.Name;
                thisNTag.UpdatedDate = DateTime.Now;
                thisNTag.CreatedBy = pcat.CreatedBy;
                thisNTag.CreatedDate = pcat.CreatedDate;
                thisNTag.UpdatedBy = accClaim.Value;
                _dbContext.NewsTags.Update(thisNTag);
            }
            _dbContext.SaveChanges();
            return Redirect($"/admin/newstag/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var sql = $"delete from NewsAndTag where NewsTagId = {id}";
            _dbContext.Database.ExecuteSqlRaw(sql);
            var query = _dbContext.NewsTags.Find(id);
            _dbContext.NewsTags.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/admin/newstag/index?page={page}");
        }
    }
}
