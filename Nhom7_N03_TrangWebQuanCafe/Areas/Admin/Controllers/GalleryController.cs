using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/gallery")]
    [Authorize(Roles = "Collaborator, Admin")]
    public class GalleryController : Controller
    {
        private readonly ILogger<GalleryController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment;
        public GalleryController(ILogger<GalleryController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Galleries.ToList();
            PagedList<Gallery> lst = new PagedList<Gallery>(query, pageNumber, pageSize);
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
        public IActionResult AddOrUpdate(int? page)
        {
            var gallery = new Gallery();
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
            return View(gallery);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(Gallery model, int? page)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/gallery/AddOrUpdate?id={model.GalleryId}&page={page}");
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var accountId = _dbContext.Accounts.Where(x => x.Username == accClaim.Value).FirstOrDefault().AccountId;

            var gallery = new Gallery()
            {
                Path = model.Path,
                Format = model.Format,
                AccountId = accountId
            };
            _dbContext.Galleries.Add(gallery);
            
            _dbContext.SaveChanges();
            return Redirect($"/Admin/gallery/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var query = _dbContext.Galleries.Find(id);
            _dbContext.Galleries.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/gallery/index?page={page}");
        }

        [Route("uploadfile")]
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Json(new { status = "error" });
            }
            string folderUploads = Path.Combine(_environment.WebRootPath, "assets\\img\\gallery-image");
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folderUploads, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string filePath = "/assets/img/gallery-image/" + fileName;
            string format = Path.GetExtension(filePath);
            return Json(new
            {
                status = "success",
                filePath,
                format
            });
        }
    }
}
