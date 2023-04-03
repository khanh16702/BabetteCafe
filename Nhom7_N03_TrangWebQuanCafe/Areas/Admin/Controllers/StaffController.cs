using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Net;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/staff")]
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public StaffController(ILogger<StaffController> logger)
        {
            _logger = logger;
        }
        [Route("Index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.staff
                .Where(x => string.IsNullOrEmpty(name) || x.FirstName.ToLower().Contains(name.Trim().ToLower()));
            PagedList<staff> lst = new PagedList<staff>(query, pageNumber, pageSize);
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
            var staffView = new staff();
            if (id > 0)
            {
                var entity = _dbContext.staff.Find(id);
                staffView = new staff()
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    DateOfBirth = entity.DateOfBirth,
                    Address = entity.Address,
                    Phone = entity.Phone
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
            return View(staffView);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(staff model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/staff/AddOrUpdate?id={model.StaffId}&page={page}");
            }
            if (model.StaffId > 0)
            {
                var query = _dbContext.staff.Find(model.StaffId);
                query.FirstName = model.FirstName;
                query.LastName = model.LastName;
                query.DateOfBirth = model.DateOfBirth;
                query.Address = model.Address;
                query.Phone = model.Phone;
                _dbContext.staff.Update(query);
            }
            else
            {
                var staffEntity = new staff()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    Phone = model.Phone
            };
                _dbContext.staff.Add(staffEntity);
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/staff/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var inSalesReceipt = _dbContext.SalesReceipts.Where(x => x.StaffId == id);
            foreach (var item in inSalesReceipt)
            {
                item.StaffId = null;
            }

            var query = _dbContext.staff.Find(id);
            _dbContext.staff.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/staff/index?page={page}");
        }
    }
}
