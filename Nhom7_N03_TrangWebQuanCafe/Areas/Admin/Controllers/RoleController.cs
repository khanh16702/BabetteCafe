using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.MinimalApi;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly ILogger<RoleController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public RoleController(ILogger<RoleController> logger)
        {
            _logger = logger;
        }

        [Route("Index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Roles
                .Where(x => string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.Trim().ToLower()));
            PagedList<Role> lst = new PagedList<Role>(query, pageNumber, pageSize);
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
            var role = new Role();
            if (id > 0)
            {
                var entity= _dbContext.Roles.Find(id);
                role.RoleId = entity.RoleId;
                role.Name = entity.Name;
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
            return View(role);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(Role model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/role/AddOrUpdate?id={model.RoleId}&page={page}");
            }
            if (model.RoleId > 0)
            {
                var query = _dbContext.Roles.Find(model.RoleId);
                query.Name = model.Name;
                _dbContext.Roles.Update(query);
            }
            else
            {
                var queryCheck = _dbContext.Roles.Where(x => x.Name == model.Name);
                if (queryCheck.Count() > 0)
                {
                    TempData["error"] = "Role already exists";
                    return Redirect($"/Admin/role/AddOrUpdate?id={model.RoleId}&page={page}");
                } 
                var role = new Role()
                {
                    Name = model.Name
                };
                _dbContext.Roles.Add(role);
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/role/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var query = _dbContext.Roles.Find(id);
            _dbContext.Roles.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/role/index?page={page}");
        }
    }
}
