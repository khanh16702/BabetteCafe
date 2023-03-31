using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/banner")]
    [Authorize(Roles = "Collaborator, Admin")]
    public class BannerController : Controller
    {
        private readonly ILogger<BannerController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment;
        public BannerController(ILogger<BannerController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Banners
                .Where(x => string.IsNullOrEmpty(name) || x.Title.ToLower().Contains(name.Trim().ToLower()))
                .Select(x => new Banner()
                {
                    BannerId = x.BannerId,
                    Title = x.Title,
                    Content = x.Content,
                    Image = x.Image,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToList();
            PagedList<Banner> lst = new PagedList<Banner>(query, pageNumber, pageSize);
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
            var banner = new Banner();
            if (id > 0)
            {
                var entity = _dbContext.Banners.Find(id);
                banner.BannerId = entity.BannerId;
                banner.Title = entity.Title;
                banner.Content = entity.Content;
                banner.Image = entity.Image;
                banner.CreatedDate = entity.CreatedDate;
                banner.UpdatedDate = entity.UpdatedDate;
                banner.CreatedBy = entity.CreatedBy;
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
            return View(banner);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(Banner model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/banner/AddOrUpdate?id={model.BannerId}&page={page}");
            }
            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (model.BannerId > 0)
            {
                var query = _dbContext.Banners.Find(model.BannerId);
                query.Title = model.Title;
                query.Content = model.Content;
                query.Image = model.Image;
                query.CreatedDate = model.CreatedDate;
                query.UpdatedDate = DateTime.Now;
                query.CreatedBy = model.CreatedBy;
                query.UpdatedBy = accClaim.Value;
                _dbContext.Banners.Update(query);
            }
            else
            {
                var banner = new Banner()
                {
                    Title = model.Title,
                    Content = model.Content,
                    Image = model.Image,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value
                };
                _dbContext.Banners.Add(banner);
            }
            _dbContext.SaveChanges();
            return Redirect($"/Admin/banner/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var query = _dbContext.Banners.Find(id);
            _dbContext.Banners.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/banner/index?page={page}");
        }

        [Route("uploadfile")]
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Json(new { status = "error" });
            }
            string folderUploads = Path.Combine(_environment.WebRootPath, "assets\\img\\banner-image");
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folderUploads, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string filePath = "/assets/img/banner-image/" + fileName;
            return Json(new
            {
                status = "success",
                filePath
            });
        }
    }
}
