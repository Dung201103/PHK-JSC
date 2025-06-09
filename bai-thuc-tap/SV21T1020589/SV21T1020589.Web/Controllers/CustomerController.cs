using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589.BusinessLayers;
using SV21T1020589.DomainModels;
using SV21T1020589.Web.Models;

namespace SV21T1020589.Web.Controllers
{
    //[Authorize] //Người sử dụng phải đăng nhập
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR}")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";

        [AllowAnonymous]
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        [AllowAnonymous]
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOffCustomers(out rowCount , condition.Page, condition.PageSize , condition.SearchValue ?? "");
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION , condition);
            return View(model);
        }

        public IActionResult Create() {
            ViewBag.Title = "Bổ sung khách hàng mới";
            var data = new Customer
            {
                CustomerID = 0,
                Islocked = false,
            };
            return View("Edit" , data);
        }
        public IActionResult Edit(int id = 0) {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer data) // string customerName , contactName , phone , email ,...
        {
            //TODO : Kiểm soát giá trị đầu vào

            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng mới" : "Cập nhật thông tin khách hàng";
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
            if(ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.CustomerID == 0)
                {
                    int id = CommonDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng !!!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCustomer(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng!!!");
                        return View("Edit", data);
                    }

                }
                return RedirectToAction("Index");
            }
            catch {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Edit", data);
            }
            
        }

        public IActionResult Delete(int id)  //GET POST
        { 
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCustomer(id);
            if (data == null) return RedirectToAction("Index");

            return View(data);
        }
    }
}
