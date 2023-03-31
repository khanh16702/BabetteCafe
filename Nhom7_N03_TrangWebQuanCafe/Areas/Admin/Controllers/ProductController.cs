using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using System.Xml.Linq;
using X.PagedList;

namespace Nhom7_N03_TrangWebQuanCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/product")]
    [Authorize(Roles = "Admin, Collaborator")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        private readonly IWebHostEnvironment _environment;
        public ProductController(ILogger<ProductController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        [Route("index")]
        public IActionResult Index(int? page, string name)
        {
            int pageSize = 10;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var query = _dbContext.Products
                .Where(x => string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.Trim().ToLower()))
                .Select(x => new ProductView()
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price, 
                    Image = x.Image,
                    Summary = x.Summary,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    Categories = ""
                })
                .ToList();
            foreach(var item in query)
            {
                var queryForCateName = _dbContext.ProductAndCategories
                    .Join(_dbContext.ProductCategories, a => a.ProductCategoryId, b => b.ProductCategoryId, (a, b) => new { a, b })
                    .Join(_dbContext.Products, x => x.a.ProductId, c => c.ProductId, (x, c) => new { x, c })
                    .Where(x => x.c.ProductId == item.ProductId)
                    .ToList();
                foreach (var cateName in queryForCateName)
                {
                    item.Categories += cateName.x.b.Name + " ; ";
                }
            }
            PagedList<ProductView> lst = new PagedList<ProductView>(query, pageNumber, pageSize);
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

        [Route("getallcategories")]
        public IActionResult GetAllCategories()
        {
            var query = _dbContext.ProductCategories;
            return Json(query);
        }

        [Route("addorupdate")]
        public IActionResult AddOrUpdate(int id, int? page)
        {
            ViewBag.page = page;
            var product = new ProductView();
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
            if (id > 0)
            {
                var entity = _dbContext.Products.Find(id);
                product = new ProductView()
                {
                    ProductId = entity.ProductId,
                    Name = entity.Name,
                    Price = entity.Price,
                    Image = entity.Image,
                    Summary = entity.Summary,
                    Description = entity.Description,
                    Quantity = entity.Quantity,
                    CreatedDate = entity.CreatedDate,
                    UpdatedDate = entity.UpdatedDate,
                    CreatedBy = entity.CreatedBy
                };

                var queryCategories = _dbContext.ProductAndCategories
                    .Join(_dbContext.ProductCategories, a => a.ProductCategoryId, b => b.ProductCategoryId, (a, b) => new { a, b })
                    .Join(_dbContext.Products, x => x.a.ProductId, c => c.ProductId, (x, c) => new { x, c })
                    .Where(x => x.c.ProductId == id)
                    .ToList();

                if (queryCategories.Count() > 0)
                {
                    foreach (var category in queryCategories)
                    {
                        product.Categories += category.x.b.Name;
                    }
                }
                return View(product);
            }
            return View(product);

        }

        [Route("addorupdate")]
        [HttpPost]
        public IActionResult AddOrUpdate(ProductView model, int? page, List<int> categoriesIdList)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect($"/Admin/product/AddOrUpdate?id={model.ProductId}&page={page}");
            }

            var claims = HttpContext.User.Identity as ClaimsIdentity;
            var accClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (model.ProductId == 0)
            {
                var checkQuery = _dbContext.Products.Where(x => x.Name == model.Name).FirstOrDefault();
                if (checkQuery != null)
                {
                    TempData["error"] = "This product has been in database already!";
                    return Redirect($"/Admin/product/AddOrUpdate?id={model.ProductId}&page={page}");
                }

                var entity = new Product()
                {
                    Name = model.Name,
                    Price = model.Price,
                    Image = model.Image,
                    Summary = model.Summary,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = accClaim.Value,
                    UpdatedBy = accClaim.Value,
                    Quantity = model.Quantity
                };
                _dbContext.Products.Add(entity);
                _dbContext.SaveChanges();

                if (categoriesIdList.Count() > 0)
                {
                    foreach (var i in categoriesIdList)
                    {
                        var addProductAndCategory = new ProductAndCategory()
                        {
                            ProductId = _dbContext.Products.Where(x => x.Name == entity.Name).FirstOrDefault().ProductId,
                            ProductCategoryId = _dbContext.ProductCategories.Find(i).ProductCategoryId
                        };
                        var sql = $"INSERT INTO ProductAndCategory VALUES ({addProductAndCategory.ProductId}, {addProductAndCategory.ProductCategoryId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);
                    }
                }
                
            }
            else
            {
                var query = _dbContext.Products.Find(model.ProductId);
                query.Name = model.Name;
                query.Price = model.Price;
                query.Image = model.Image;
                query.Summary = model.Summary;
                query.Description = model.Description;
                query.CreatedDate = model.CreatedDate;
                query.UpdatedDate = DateTime.Now;
                query.Quantity = model.Quantity;
                query.CreatedBy = model.CreatedBy;
                query.UpdatedBy = accClaim.Value;
                _dbContext.Products.Update(query);

                var sqlRemove = $"delete from ProductAndCategory where ProductId = {model.ProductId}";
                _dbContext.Database.ExecuteSqlRaw(sqlRemove);

                if (categoriesIdList.Count() > 0)
                {
                    foreach (var i in categoriesIdList)
                    {
                        var addProductAndCategory = new ProductAndCategory()
                        {
                            ProductId = _dbContext.Products.Where(x => x.Name == model.Name).FirstOrDefault().ProductId,
                            ProductCategoryId = _dbContext.ProductCategories.Find(i).ProductCategoryId
                        };
                        var sql = $"INSERT INTO ProductAndCategory VALUES ({addProductAndCategory.ProductId}, {addProductAndCategory.ProductCategoryId})";
                        _dbContext.Database.ExecuteSqlRaw(sql);
                    }
                }

            }
            _dbContext.SaveChanges();
            return Redirect($"/admin/product/index?page={page}");
        }

        [Route("uploadfile")]
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
            {
                return Json(new { status = "error" });
            }
            string folderUploads = Path.Combine(_environment.WebRootPath, "assets\\img\\product-image");
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folderUploads, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            string filePath = "/assets/img/product-image/" + fileName;
            return Json(new
            {
                status = "success",
                filePath
            });
        }

        [Route("delete")]
        public IActionResult Delete(int id, int? page)
        {
            var queryCheck = _dbContext.Products
                .Join(_dbContext.Carts, a => a.ProductId, b => b.ProductId, (a, b) => new { a, b })
                .Join(_dbContext.SalesReceipts, x => x.b.SalesReceiptId, c => c.SalesReceiptId, (x, c) => new { x, c })
                .Where(p => p.x.a.ProductId == id)
                .ToList();
            if (queryCheck.Count() > 0)
            {
                TempData["error"] = "Cannot remove this product";
                return Redirect($"/admin/product/index?page={page}");
            }

            var deleteInCart = _dbContext.Carts.Where(x => x.ProductId == id);
            _dbContext.Carts.RemoveRange(deleteInCart);

            var deleteProduct = _dbContext.Products.Find(id);
            _dbContext.Products.Remove(deleteProduct);

            var deleteProductsCategories = _dbContext.ProductAndCategories.Where(x => x.ProductId == id).ToList();
            foreach (var c in deleteProductsCategories)
            {
                var sql = $"delete from ProductAndCategory where ProductId = {c.ProductId}";
                _dbContext.Database.ExecuteSqlRaw(sql);
            }

            _dbContext.SaveChanges();
            return Redirect($"/admin/product/index?page={page}");

        }
    }
}
