using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589.BusinessLayers;
using SV21T1020589.DomainModels;
using SV21T1020589.Web.Models;

namespace SV21T1020589.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR} ,{WebUserRoles.EMPLOYEE}")]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";

        [AllowAnonymous]
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOffShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung giao hàng mới";
            var data = new Shipper
            {
                ShipperID = 0
            };
            return View("Edit" , data);
        }
        
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }
        
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            //TODO : Kiểm soát giá trị đầu vào
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng mới" : "Cập nhật thông tin người giao hàng";
            //Kiểm tra dữu liệu dầu vào kjhoong hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại người giao hàng không được để trống");

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng !!!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                    if (result == false) {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng!!!");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Edit", data);
            }

            
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetShipper(id);
            if (data == null) return RedirectToAction("Index");

            return View(data);
        }
    }
}
