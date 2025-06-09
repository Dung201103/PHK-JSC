using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589CLIENT.BusinessLayers;
using SV21T1020589CLIENT.DomainModels;
using Microsoft.AspNetCore.Authorization;

namespace SV21T1020589CLIENT.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
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
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            //kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }

            // Kiểm tra xem username và pass hợp lệ
            var userAccount = CustomerDataService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại , Sai Email hoặc Mật Khẩu !!");
                return View();
            }

            //Đăng nhập thành công : Ghi nhận trạng thái đăng nhập
            //1. Tạo ra thông tin của người dùng
            var userData = new WebUserData()
            {
                CustomerID = userAccount.CustomerID.ToString(),
                CustomerName = userAccount.CustomerName,
                ContactName = userAccount.ContactName,
                Province = userAccount.Province,
                Phone = userAccount.Phone,
                Address = userAccount.Address
            };

            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quya laij trang chur
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            var data = new Customer
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View(data);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(Customer data) // string customerName , contactName , phone , email ,...
        {

            //Kiểm tra dữu liệu dầu vào kjhoong hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Chọn tỉnh thành cho khách hàng");

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("Register", data);
            }

            try
            {
                if (data.CustomerID == 0)
                {
                    int id = CustomerDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng !!!");
                        return View("Register", data);
                    }
                    else
                    {
                        ModelState.AddModelError("Success", "Đăng Ký Thành Công !!!!");
                        return View("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "Đăng Ký Thất Bại!!!!");
                    return View("Register");
                }
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Register", data);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult UpdateSave(Customer data) // string customerName , contactName , phone , email ,...
        {
            //Kiểm tra dữu liệu dầu vào kjhoong hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Chọn tỉnh thành cho khách hàng");

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("EditProfile", data);
            }

            try
            {
                if (data.CustomerID != 0)
                {
                    bool result = CustomerDataService.UpdateCustomer(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng!!!");
                        return View("EditProfile", data);
                    }
                    else
                    {
                        ModelState.AddModelError("Success", "Cập Nhật Thành Công !!!!");
                        return View("EditProfile", data);
                    }

                }
                else
                {
                    ModelState.AddModelError("Error", "Cập Nhật Thất Bại , Vui Lòng Thử Lại Sau !!!");
                    return View("EditProfile");
                }

            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Register", data);
            }

        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult EditProfile(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CustomerDataService.GetCustomer(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
/*            if (string.IsNullOrWhiteSpace(oldPassword))
                ModelState.AddModelError("oldPassword", "Mật khẩu hiện tại không được để trống");
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Mật khẩu mới không được để trống");
            if (string.IsNullOrWhiteSpace(confirmPassword))
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không được để trống");*/

            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Mật khẩu mới không trúng khớp");

            if (!ModelState.IsValid)
                return View();

            var data = User.GetUserData();

            bool result = CustomerDataService.ChangePassword(data.CustomerID, oldPassword, newPassword);
            if (!result)
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
            if (!ModelState.IsValid)
                return View();
            ModelState.AddModelError("Success", "Thay Đổi Mật Khẩu Thành Công");
            return View("Login");
        }

    public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
