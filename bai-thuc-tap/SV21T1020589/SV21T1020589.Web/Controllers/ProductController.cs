using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589.BusinessLayers;
using SV21T1020589.DomainModels;
using SV21T1020589.Web.Models;
using System.Buffers;

namespace SV21T1020589.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR} ,{WebUserRoles.EMPLOYEE}")]
    public class ProductController : Controller
    {
        public const int PAGE_SIZE = 50;
        private const string PRODUCT_SEARCH_CONDITION = "ProductsSearchCondition";

        [AllowAnonymous]
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            return View(condition);
        }

        [AllowAnonymous]
        public IActionResult Search(ProductSearchInput condition)
        {

            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "" , 
                condition.CategoryID , condition.SupplierID , condition.MinPrice , condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID ,
                SupplierID = condition.SupplierID ,
                MinPrice = condition.MinPrice ,
                Data = data
                
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng mới";
            var data = new Product
            {
                ProductID = 0,
                IsSelling = true,
                Photo = "noimage.png"
            };
           
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            var data = ProductDataService.GetProduct(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }


        [HttpPost]
        public IActionResult Save(Product data, IFormFile? _Photo) // string customerName , contactName , phone , email ,...
        {
            //TODO : Kiểm soát giá trị đầu vào

            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";
            //Kiểm tra dữu liệu dầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            //if (string.IsNullOrWhiteSpace(data.ProductDescription))
            // ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả mặt hàng không được để trống");
            //if (string.IsNullOrWhiteSpace(data.CategoryID.ToString()) )
            //    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng !");
            //if (string.IsNullOrWhiteSpace(data.SupplierID.ToString()))
            //    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
            if (data.CategoryID == 0)
            {
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            }
            if (data.SupplierID == 0)
            {
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
            }

            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.Price.ToString()))
            {
                ModelState.AddModelError(nameof(data.Price), "Giá không được để trống");
            }
            if (decimal.TryParse(data.Price.ToString(), out var price)){
                if (price < 0)
                {
                    ModelState.AddModelError(nameof(data.Price), "Giá không được là số âm");
                }
            }
            



            //Xu ly với ảnh
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, "images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng bị trùng !!!");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng bị trùng !!!");
                        return View("Edit", data);
                    }
                }
                var reUrl = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(reUrl))
                {
                    return Redirect(reUrl);
                }
                else return RedirectToAction("Index");
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
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var data = ProductDataService.GetProduct(id);
            if (data == null) return RedirectToAction("Index");

            return View(data);
        }



      
        public IActionResult Photo(int id = 0 , string method ="" ,int photoId = 0)
        {
            var reUrl = Request.Headers["Referer"].ToString(); //code trở về trang trước
            switch (method) {
                case "add":
                    //ViewBag.Title = "Bổ sung ảnh mặt hàng";
                    var dataPhotos = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = id,
                        IsHidden = false,
                        DisplayOrder = 0,
                        Photo = "noimage.png"
                    };
                    return View("Photo" , dataPhotos);
                case "edit":
                    //ViewBag.Title = "Thay đổi ảnh mặt hàng";
                    var data = ProductDataService.GetPhoto(photoId);
                    if (data == null) return RedirectToAction("Index");
                    return View("Photo", data);
                   
                case "delete":
                    //TODO:xóa trực tiếp k cần confirm
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit" , new {id = id});
                default:
                    if (!string.IsNullOrEmpty(reUrl))
                    {
                        return Redirect(reUrl);
                    }
                    else return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult PhotoSave (ProductPhoto data, IFormFile? uploadPhoto)
        {
            //TODO : Kiểm soát giá trị đầu vào

            ViewBag.Title = data.ProductID == 0 ? "Bổ sung ảnh mặt hàng mới" : "Cập nhật thông tin ảnh mặt hàng";
            //Kiểm tra dữu liệu dầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");    

            //Xu ly với ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, "images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("Photo", data);
            }

            try
            {
                if (data.PhotoID == 0)
                {
                    long id = ProductDataService.AddPhoto(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Description), "Mô tả bị trùng !!!");
                        return View("Photo", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Description), "Mô tả bị trùng !!!");
                        return View("Photo", data);
                    }
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Photo", data);
            }
        }


        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính mặt hàng";
                    var dataAttribute = new ProductAttribute
                    {
                        AttributeID = 0,
                        ProductID = id,
                        DisplayOrder = 0
                    };
                    return View("Attribute", dataAttribute);
                   
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính mặt hàng";
                    var data = ProductDataService.GetAttribute(attributeId);
                    if (data == null) return RedirectToAction("Index");
                    return View("Attribute", data);
                  
                case "delete":
                    //TODO:xóa trực tiếp k cần confirm
                    ProductDataService.DelAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult AttributeSave(ProductAttribute data)
        {
            //TODO : Kiểm soát giá trị đầu vào

            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính mặt hàng mới" : "Cập nhật thông tin thuộc tính mặt hàng";
            //Kiểm tra dữu liệu dầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào modelState
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Gía trị không được để trống");


            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay k?
            if (ModelState.IsValid == false)
            {
                return View("Attribute", data);
            }

            try
            {
                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính bị trùng !!!");
                        return View("Attribute", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính bị trùng !!!");
                        return View("Attribute", data);
                    }
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống bị gián đoạn");
                return View("Attribute", data);
            }
        }
    }
}
