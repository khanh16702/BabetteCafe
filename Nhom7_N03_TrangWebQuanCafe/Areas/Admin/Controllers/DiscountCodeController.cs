using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/discountcode")]
    [Authorize(Roles = "Admin, Collaborator")]
    public class DiscountCodeController : Controller
    {
        private readonly ILogger<DiscountCodeController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public DiscountCodeController(ILogger<DiscountCodeController> logger)
        {
            _logger = logger;
        }
        [Route("Index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.DiscountCodes
                .Where(x => string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.Trim().ToLower()));
            PagedList<DiscountCode> lst = new PagedList<DiscountCode>(query, pageNumber, pageSize);
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
            var discountCode = new DiscountCode();
            if (id > 0)
            {
                var entity = _dbContext.DiscountCodes.Find(id);

                if (DateTime.Compare((DateTime)entity.ActiveDate, DateTime.Now) <= 0)
                {
                    TempData["error"] = "Cannot update this discount!";
                    return Redirect($"/Admin/discountcode/index?page={page}");
                }

                discountCode.DiscountCodeId = entity.DiscountCodeId;
                discountCode.Name = entity.Name;
                discountCode.DecreaseAmount = entity.DecreaseAmount;
                discountCode.MinimumToApply = entity.MinimumToApply;
                discountCode.CreatedBy = entity.CreatedBy;
                discountCode.CreatedDate = entity.CreatedDate;
                discountCode.ActiveDate = entity.ActiveDate;
                discountCode.ExpireDate = entity.ExpireDate;
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
            return View(discountCode);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(DiscountCode model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/discountcode/AddOrUpdate?id={model.DiscountCodeId}&page={page}");
            }

            if (DateTime.Compare((DateTime)model.ActiveDate,DateTime.Now) <= 0)
            {
                TempData["Error"] = "Active date is not valid";
                return Redirect($"/Admin/discountcode/AddOrUpdate?id={model.DiscountCodeId}&page={page}");
            }

            if (DateTime.Compare((DateTime)model.ActiveDate, (DateTime)model.ExpireDate) >= 0
                || DateTime.Compare((DateTime)model.ExpireDate, DateTime.Now) <= 0)
            {
                TempData["Error"] = "Expire date is not valid";
                return Redirect($"/Admin/discountcode/AddOrUpdate?id={model.DiscountCodeId}&page={page}");
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (model.DiscountCodeId > 0)
            {
                var query = _dbContext.DiscountCodes.Find(model.DiscountCodeId);
                query.Name = model.Name;
                query.MinimumToApply = model.MinimumToApply;
                query.DecreaseAmount = model.DecreaseAmount;
                query.CreatedBy = model.CreatedBy;
                query.CreatedDate = model.CreatedDate;
                query.ActiveDate = model.ActiveDate;
                query.ExpireDate = model.ExpireDate;
                _dbContext.DiscountCodes.Update(query);
            }
            else
            {
                var queryCheck = _dbContext.DiscountCodes.Where(x => x.Name == model.Name);
                if (queryCheck.Count() > 0)
                {
                    TempData["error"] = "Discount code already exists";
                    return Redirect($"/Admin/discountcode/AddOrUpdate?id={model.DiscountCodeId}&page={page}");
                }
                var discountCode = new DiscountCode()
                {
                    DecreaseAmount = model.DecreaseAmount,
                    MinimumToApply = model.MinimumToApply,
                    Name = model.Name,
                    CreatedBy = accClaim.Value,
                    CreatedDate = DateTime.Now,
                    ActiveDate = model.ActiveDate,
                    ExpireDate = model.ExpireDate
                };
                _dbContext.DiscountCodes.Add(discountCode);
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/discountcode/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var queryCheck = _dbContext.DiscountCodes.Find(id);
            if (DateTime.Compare((DateTime)queryCheck.ActiveDate, DateTime.Now) <= 0)
            {
                TempData["error"] = "Cannot remove this discount code";
                return Redirect($"/Admin/discountcode/index?page={page}");
            } 

            var query = _dbContext.DiscountCodes.Find(id);
            _dbContext.DiscountCodes.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/discountcode/index?page={page}");
        }
    }
}
