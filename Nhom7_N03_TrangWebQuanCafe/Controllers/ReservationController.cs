using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ILogger<ReservationController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public ReservationController(ILogger<ReservationController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (accClaim != null)
            {
                ViewBag.CurrentUsername = accClaim.Value;
                var accId = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().AccountId;
                var accRole = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().RoleId;
                var cartCount = _dbContext.Carts
                    .Where(x => x.AccountId == accId && x.IsPlaced == false)
                    .ToList();
                ViewBag.CartCount = cartCount.Count();
                ViewBag.accRole = accRole;
            }
            else
            {
                ViewBag.CurrentUsername = "";
                ViewBag.CartCount = 0;
            }

            var query = _dbContext.Galleries.ToList();
            return View(query);
        }

        public IActionResult FindTable(int numberOfPeople, int bookedTime)
        {
            var queryTables = _dbContext.TableShops.Where(x => x.Slots == numberOfPeople).ToList();
            var queryBookTimes = _dbContext.BookTimes.ToList();
            var a = queryTables.Count() / (queryBookTimes.Count() - 1);
            var queryCheck = _dbContext.TableShops
                .Where(x => x.BookTimeId == bookedTime && x.Slots == numberOfPeople)
                .ToList();
            if (queryCheck.Count() == a)
            {
                return Json(new { status = "fail" });
            }
            var query = _dbContext.TableShops
                .Where(x => x.Slots == numberOfPeople && x.Status == "Available")
                .FirstOrDefault();
            if (query != null)
            {
                query.Status = "In Use";
                query.BookTimeId = bookedTime;
                _dbContext.TableShops.Update(query);

                var claims = HttpContext.User.Identity as ClaimsIdentity;
                var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
                var accId = _dbContext.Accounts
                    .Where(x => x.Username == accClaim.Value)
                    .FirstOrDefault().AccountId;

                var sql = $"insert into TableShopAndAccount values ({query.TableId}, {accId})";
                _dbContext.Database.ExecuteSqlRaw(sql);

                _dbContext.SaveChanges();

                return Json(new { status = "success"});
            }
            else
            {
                return Json(new { status = "fail" });
            }
        }
    }
}
