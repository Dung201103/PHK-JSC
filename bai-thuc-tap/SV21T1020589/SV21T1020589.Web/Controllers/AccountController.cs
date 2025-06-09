using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589.BusinessLayers;

namespace SV21T1020589.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username , string password)
        {
            ViewBag.Username = username;
            //kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }

            // Kiểm tra xem username và pass hợp lệ
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null) 
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Đăng nhập thành công : Ghi nhận trạng thái đăng nhập
            //1. Tạo ra thông tin của người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quya laij trang chur
            return RedirectToAction("Index" , "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }

        //public IActionResult ChangePassword()
        //{
        //    return View();
        //}

        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
          

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Mật khẩu mới không được để trống");

            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu mới không trúng khớp");

            if (!ModelState.IsValid)
                return View();

            var data = User.GetUserData();

            bool result = UserAccountService.ChangePassword(data.UserName, oldPassword, newPassword);
            if (!result)
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
            if (!ModelState.IsValid)
                return View();
            return RedirectToAction("Login");
        }
    }
}
