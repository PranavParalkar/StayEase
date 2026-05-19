using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StayEase.BUS;

namespace StayEase.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService = new();

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var account = _authService.Login(username, password);
            if (account == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            string empName = _authService.GetEmployeeName(account.EmployeeID);
            string roleName = _authService.GetRoleName(account.RoleID);
            var permissions = _authService.GetRolePermissions(account.RoleID);
            string permStr = string.Join(",", permissions.Select(p => p.FeatureID));

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, empName),
                new("Username", account.Username),
                new("EmployeeID", account.EmployeeID),
                new("RoleID", account.RoleID),
                new(ClaimTypes.Role, roleName),
                new("Permissions", permStr)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
