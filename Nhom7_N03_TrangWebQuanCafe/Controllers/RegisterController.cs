using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nhom7_N03_TrangWebQuanCafe.Classes;
using Nhom7_N03_TrangWebQuanCafe.Models;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    [Route("/register")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("registernewaccount")]
        [HttpPost]
        public IActionResult RegisterNewAccount (AccountView model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                TempData["Error"] = error.FirstOrDefault();
                return Redirect("/register/index");
            }

            if (model.Password != model.RetypedPassword)
            {
                TempData["Error"] = "Retyped password is incorrect!";
                return Redirect("/register/index"); 
            }

            var queryCheck = _dbContext.Accounts.Where(x => x.Username == model.Username).FirstOrDefault();
            if (queryCheck != null)
            {
                TempData["Error"] = "Username already exists!";
                return Redirect("/register/index");
            }

            var account = new Account()
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                DisplayName = model.DisplayName,
                Image = "~/assets/img/default.jpg",
                RoleId = 6  
            };

            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();

            return Redirect("/home/index");
        }
    }
}
