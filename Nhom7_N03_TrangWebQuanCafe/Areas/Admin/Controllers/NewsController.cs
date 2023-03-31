using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/news")]
    [Authorize(Roles = "Admin, Collaborator")]
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment;
        public NewsController(ILogger<NewsController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.News
                .Where(x => string.IsNullOrEmpty(name) || x.Title.ToLower().Contains(name.Trim().ToLower()))
                .ToList();
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



            List<News> news = new List<News>();
            if (currentRoleName != "Admin")
            {
                foreach(var item in query)
                {
                    if (item.AccountId == thisAcc.AccountId)
                    {
                        news.Add(item);
                    }
                }
            }
            else
            {
                news = query;
            }

            List<NewsView> newsView = new List<NewsView>();

            foreach (var item in news)
            {
                var nV = new NewsView()
                {
                    NewsId = item.NewsId,
                    Title = item.Title,
                    Summary = item.Summary,
                    Content = item.Content,
                    IsIntroduction = item.IsIntroduction,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                    AccountId = item.AccountId,
                    Image = item.Image
                };
                
                var queryGetCategories = _dbContext.NewsAndCategories
                    .Join(_dbContext.NewsCategories, a => a.NewsCategoryId, b => b.NewsCategoryId, (a, b) => new { a, b })
                    .Where(x => x.a.NewsId == item.NewsId)
                    .ToList();
                string nVCategories = "";
                foreach (var category in queryGetCategories)
                {
                    nVCategories += category.b.Name + " ; ";
                }
                nV.Categories = nVCategories;

                var queryGetTags = _dbContext.NewsAndTags
                    .Join(_dbContext.NewsTags, a => a.NewsTagId, b => b.NewsTagId, (a, b) => new { a, b })
                    .Where(x => x.a.NewsId == item.NewsId)
                    .ToList();
                string nVTags = "";
                foreach(var tag in queryGetTags)
                {
                    nVTags += tag.b.Name + " ; ";
                }
                nV.Tags = nVTags;

                newsView.Add(nV);
            }

            PagedList<NewsView> lst = new PagedList<NewsView>(newsView, pageNumber, pageSize);

            return View(lst);
        }

        [Route("addorupdate")]
        public IActionResult AddOrUpdate(int id, int? page)
        {
            var news = new NewsView();
            if (id > 0)
            {
                var entity = _dbContext.News.Find(id);
                news.NewsId = entity.NewsId;
                news.Title = entity.Title;
                news.Summary = entity.Summary;
                news.Content = entity.Content;
                news.IsIntroduction = entity.IsIntroduction;
                news.CreatedDate = entity.CreatedDate;
                news.AccountId = entity.AccountId;
                news.Image = entity.Image;

                var queryCategories = _dbContext.NewsAndCategories
                    .Join(_dbContext.NewsCategories, a => a.NewsCategoryId, b => b.NewsCategoryId, (a, b) => new { a, b })
                    .Where(x => x.a.NewsId == id)
                    .ToList();
                foreach(var category in queryCategories)
                {
                    news.Categories += category.b.Name + " ; ";
                }

                var queryTags = _dbContext.NewsAndTags
                    .Join(_dbContext.NewsTags, a => a.NewsTagId, b => b.NewsTagId, (a, b) => new { a, b })
                    .Where(x => x.a.NewsId == id)
                    .ToList();
                foreach (var tag in queryTags)
                {
                    news.Tags += tag.b.Name + " ; ";
                }
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
            return View(news);
        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(News model, int? page, List<int> categoriesIdList, List<int> tagsIdList)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/news/AddOrUpdate?id={model.NewsId}&page={page}");
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            var getAccountId = _dbContext.Accounts.Where(x => x.Username == accClaim.Value).FirstOrDefault().AccountId;

            if (model.NewsId > 0)
            {
                var query = _dbContext.News.Find(model.NewsId);

                var reduceQuantityQuery = _dbContext.NewsAndCategories
                    .Join(_dbContext.NewsCategories, a => a.NewsCategoryId, b => b.NewsCategoryId, (a, b) => new { a, b })
                    .Where(x => x.a.NewsId == query.NewsId)
                    .ToList();
                foreach (var item in reduceQuantityQuery)
                {
                    item.b.NumberOfPosts = Math.Max((int)(item.b.NumberOfPosts - 1), 0);
                    _dbContext.NewsCategories.Update(item.b);
                }

                query.Title = model.Title;
                query.Summary = model.Summary;
                query.Content = model.Content;
                query.IsIntroduction = model.IsIntroduction;
                query.CreatedDate = model.CreatedDate;
                query.UpdatedDate = DateTime.Now;
                query.UpdatedBy = accClaim.Value;
                query.UpdatedDate = DateTime.Now;
                query.AccountId = model.AccountId;
                query.Image = model.Image;
                _dbContext.News.Update(query);

                var sqlRemove = $"delete from NewsAndCategory where NewsId = {model.NewsId}";
                _dbContext.Database.ExecuteSqlRaw(sqlRemove);

                if (categoriesIdList.Count() > 0)
                {
                    foreach (var i in categoriesIdList)
                    {
                        var addNewsAndCategory = new NewsAndCategory()
                        {
                            NewsId = _dbContext.News.Where(x => x.Title == model.Title).FirstOrDefault().NewsId,
                            NewsCategoryId = _dbContext.NewsCategories.Find(i).NewsCategoryId
                        };
                        var sql = $"INSERT INTO NewsAndCategory VALUES ({addNewsAndCategory.NewsId}, {addNewsAndCategory.NewsCategoryId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);

                        var increaseQuantityQuery = _dbContext.NewsCategories.Find(i);
                        increaseQuantityQuery.NumberOfPosts += 1;
                        _dbContext.NewsCategories.Update(increaseQuantityQuery);
                    }
                }

                var sqlRemoveTags = $"delete from NewsAndTag where NewsId = {model.NewsId}";
                _dbContext.Database.ExecuteSqlRaw(sqlRemoveTags);

                if (tagsIdList.Count() > 0)
                {
                    foreach (var i in tagsIdList)
                    {
                        var addNewsAndTag = new NewsAndTag()
                        {
                            NewsId = _dbContext.News.Where(x => x.Title == model.Title).FirstOrDefault().NewsId,
                            NewsTagId = _dbContext.NewsTags.Find(i).NewsTagId
                        };
                        var sql = $"INSERT INTO NewsAndTag VALUES ({addNewsAndTag.NewsId}, {addNewsAndTag.NewsTagId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);
                    }
                }
            }
            else
            {
                var checkQuery = _dbContext.News.Where(x => x.Title == model.Title).FirstOrDefault();
                if (checkQuery != null)
                {
                    TempData["error"] = "This news has been in database already!";
                    return Redirect($"/Admin/news/AddOrUpdate?id={model.NewsId}&page={page}");
                }
                bool isIntro = false;
                if (model.IsIntroduction.HasValue) {
                    isIntro = model.IsIntroduction.Value;
                }
                var news = new News()
                {
                    Title = model.Title,
                    Summary = model.Summary,
                    Content = model.Content,
                    IsIntroduction = isIntro,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = accClaim.Value,
                    Image = model.Image,
                    AccountId = getAccountId
                };
                _dbContext.News.Add(news);
                _dbContext.SaveChanges();

                if (categoriesIdList.Count() > 0)
                {
                    foreach (var i in categoriesIdList)
                    {
                        var addNewsAndCategory = new NewsAndCategory()
                        {
                            NewsId = _dbContext.News.Where(x => x.Title == model.Title).FirstOrDefault().NewsId,
                            NewsCategoryId = _dbContext.NewsCategories.Find(i).NewsCategoryId
                        };
                        var sql = $"INSERT INTO NewsAndCategory VALUES ({addNewsAndCategory.NewsId}, {addNewsAndCategory.NewsCategoryId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);

                        var increaseQuantityQuery = _dbContext.NewsCategories.Find(i);
                        increaseQuantityQuery.NumberOfPosts += 1;
                        _dbContext.NewsCategories.Update(increaseQuantityQuery);
                    }
                }

                if (tagsIdList.Count() > 0)
                {
                    foreach (var i in tagsIdList)
                    {
                        var addNewsAndTag = new NewsAndTag()
                        {
                            NewsId = _dbContext.News.Where(x => x.Title == news.Title).FirstOrDefault().NewsId,
                            NewsTagId = _dbContext.NewsTags.Find(i).NewsTagId
                        };
                        var sql = $"INSERT INTO NewsAndTag VALUES ({addNewsAndTag.NewsId}, {addNewsAndTag.NewsTagId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);
                    }
                }
            }

            if (model.IsIntroduction.HasValue && model.IsIntroduction.Value)
            {
                var changeIsIntroduction = _dbContext.News.Where(x => x.Title != model.Title).ToList();
                foreach (var item in changeIsIntroduction)
                {
                    item.IsIntroduction = false;
                    _dbContext.News.Update(item);
                }
            }

            _dbContext.SaveChanges();
            return Redirect($"/Admin/news/index?page={page}");
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var reduceNumberOfPosts = _dbContext.NewsAndCategories
                .Join(_dbContext.NewsCategories, a => a.NewsCategoryId, b => b.NewsCategoryId, (a, b) => new { a, b })
                .Where(x => x.a.NewsId == id)
                .ToList();
            foreach (var c in reduceNumberOfPosts)
            {
                c.b.NumberOfPosts -= 1;
                _dbContext.NewsCategories.Update(c.b);
            }

            var deleteNewsAndCategories = _dbContext.NewsAndCategories.Where(x => x.NewsId == id).ToList();
            foreach (var c in deleteNewsAndCategories)
            {
                var sql = $"delete from NewsAndCategory where NewsId = {c.NewsId}";
                _dbContext.Database.ExecuteSqlRaw(sql);
            }

            var deleteNewsAndTags = _dbContext.NewsAndTags.Where(x => x.NewsId == id).ToList();
            foreach (var c in deleteNewsAndTags)
            {
                var sql = $"delete from NewsAndTag where NewsId = {c.NewsId}";
                _dbContext.Database.ExecuteSqlRaw(sql);
            }

            var query = _dbContext.News.Find(id);
            _dbContext.News.Remove(query);
            _dbContext.SaveChanges();
            return Redirect($"/Admin/news/index?page={page}");
        }

        [Route("uploadfile")]
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Json(new { status = "error" });
            }
            string folderUploads = Path.Combine(_environment.WebRootPath, "assets\\img\\news-image");
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folderUploads, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string filePath = "/assets/img/news-image/" + fileName;
            return Json(new
            {
                status = "success",
                filePath
            });
        }

        [Route("getallnewscategories")]
        public IActionResult GetAllNewsCategories()
        {
            return Json(_dbContext.NewsCategories.ToList());
        }

        [Route("getallnewstags")]
        public IActionResult GetAllNewsTags()
        {
            return Json(_dbContext.NewsTags.ToList());
        }
    }
}
