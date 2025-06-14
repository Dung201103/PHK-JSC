﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589.BusinessLayers;
using SV21T1020589.DomainModels;
using SV21T1020589.Web.Models;

namespace SV21T1020589.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class CategoryController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOffCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng mới";
            var data = new Category
            {
                CategoryID = 0
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var data = CommonDataService.GetCategory(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }
         
        [HttpPost]
        public IActionResult Save(Category data) 
        {
            //TODO : Kiểm soát giá trị đầu vào
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";
            //Kiểm tra dữ liệu đầu vào.
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName),"Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Tên mặt hàng không được để trống");

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false) {
                return View("Edit", data);
            }
            try
            {
                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên bị trùng !!!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên bị trùng!!!");
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
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCategory(id);
            if (data == null) return RedirectToAction("Index");

            return View(data);
        }
    }
}
