using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/table")]
    [Authorize(Roles = "Admin, Collaborator, Staff")]
    public class TableController : Controller
    {
        private readonly ILogger<TableController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public TableController(ILogger<TableController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.TableShops.ToList();
            PagedList<TableShop> lst = new PagedList<TableShop>(query, pageNumber, pageSize);
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
            var table = new TableShop();
            if (id > 0)
            {
                var entity = _dbContext.TableShops.Find(id);
                table.TableId = entity.TableId;
                table.Slots = entity.Slots;
                table.Status = entity.Status;
                table.BookTimeId = entity.BookTimeId;
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
            return View(table);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(TableShop model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/table/AddOrUpdate?id={model.TableId}&page={page}");
            }
            if (model.TableId > 0)
            {
                var table = _dbContext.TableShops.Find(model.TableId);
                table.Slots = model.Slots;
                table.Status = model.Status;
                table.BookTimeId = model.BookTimeId;
                _dbContext.TableShops.Update(table);
            }
            else
            {
                var countBookTime = _dbContext.BookTimes.ToList().Count();
                for (var i = 0; i < countBookTime-1; i++)
                {
                    var table = new TableShop()
                    {
                        Slots = model.Slots,
                        Status = "Available",
                        BookTimeId = -1
                    };
                    _dbContext.TableShops.Add(table);
                }
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/table/index?page={page}");
        }
    }
}
