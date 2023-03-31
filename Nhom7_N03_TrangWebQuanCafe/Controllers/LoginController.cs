using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Nhom7_N03_TrangWebQuanCafe.Models;
using System.Security.Claims;
using Nhom7_N03_TrangWebQuanCafe.Classes;
using System.Configuration;

namespace Nhom7_N03_TrangWebQuanCafe.Controllers
{
    [Route("/login")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        CafeWebsiteContext _dbContext = new CafeWebsiteContext();
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("loginaccount")]
        [HttpPost]
        public async Task<IActionResult> LoginAccount(Account model)
        {
            
            if (model.Password == null)
            {
                TempData["Error"] = "Password input is blank!";
                return Redirect("/login/index");
            }
            string encodedPassword = model.Password;
            var query = _dbContext.Accounts
                .Where(x => x.Username == model.Username)
                .Where(x => x.Password == encodedPassword)
                .Select(x => new Account()
                {
                    AccountId = x.AccountId,
                    Username = x.Username,
                    Password = x.Password
                });
            if (query.Count() == 0)
            {
                TempData["Error"] = "Username or password is not correct";
                return Redirect("/login/index");
            }
            else
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, model.Username.ToLower())
                };

                var returnURL = "";
                var roleId = _dbContext.Accounts
                    .Where(x => x.Username == model.Username)
                    .FirstOrDefault().RoleId;
                var roleName = _dbContext.Roles.Find(roleId).Name;
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                switch (roleId)
                {
                    case 1:
                        returnURL = "/admin/home/index";
                        break;
                    case 2:
                        returnURL = "/admin/customer/index";
                        break;
                    case 3:
                        returnURL = "/admin/product/index";
                        break;
                    default:
                        returnURL = "/home/index";
                        break;
                }

                var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                return Redirect(returnURL);
            }
        }

        [Route("signout")]
        public new async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login/index");
        }

        [Route("forbidden")]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
