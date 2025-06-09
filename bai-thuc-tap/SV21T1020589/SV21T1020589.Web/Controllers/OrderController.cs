using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using SV21T1020589.BusinessLayers;
using SV21T1020589.DomainModels;
using SV21T1020589.Web.Models;
using System.Globalization;

namespace SV21T1020589.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTRATOR} ,{WebUserRoles.EMPLOYEE}")]
    public class OrderController : Controller
    {
        public const int PAGE_SIZE = 50;

        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        private const string PRODUCT_SEARCH_CONDITION = "Products_Order_SearchCondition";
        private const string SHOPPING_CART = "ShoppingCart";


        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-1000).ToString("dd/MM/yyyy",cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy",cultureInfo)}"
                };
            }
            return View(condition);
        }

        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status ,
                condition.FromTime , condition.ToTime , condition.SearchValue ?? "");

            OrderSearchResult model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE/10,
                    SearchValue = "",
                };
            }
            return View(condition);
        }

        public IActionResult SearchProduct (PaginationSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts (out rowCount, condition.Page, condition.PageSize , condition.SearchValue ?? "");

            ProductSearchOrderResult model = new ProductSearchOrderResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }

        // Cart function
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null) 
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity < 0)
                return Json("Giá bán và số lượng không hợp lệ");
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice += item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART , shoppingCart);
            return Json("");
            //return RedirectToAction("Create");
        }

        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if(index > 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }

        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            }

            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress)) 
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            }
			//int employeeID = 1;  //TODO : Thay bở ID của nhân viên đang login vào hệ thông sau khi làm phần Đăng Nhập
			var userData = User.GetUserData();
            int employeeID = 1;
			if (userData != null)
            {
                employeeID = int.Parse(userData.UserId);
            }

			List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail { 
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID , customerID , deliveryProvince , deliveryAddress , orderDetails);
            ClearCart();
            return Json(orderID);
        }
        // End Cart Function

        public IActionResult Details(int id = 0)
        {
            var data = OrderDataService.GetOrder(id);
            if (data == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = data,
                Details = details
            };
            return View(model);
        }
        
        public IActionResult EditDetail(int id = 0 , int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id , productId);
            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            ViewBag.Title = "Quản lý đơn hàng";
            OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            return RedirectToAction("Details" , new { id = orderID });

        }


        [HttpPost]
        public IActionResult DeleteDetail(int orderId, int productId)
        {
            try
            {  
                // Xóa sản phẩm khỏi đơn hàng
                var result = OrderDataService.DeleteOrderDetail(orderId, productId);
                if(result)
                    // Trả về kết quả JSON
                    return Json(new { success = true, message = "Sản phẩm đã được xóa thành công." });
                else
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm."});

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm.", error = ex.Message });
            }
        }

        public IActionResult Accept(int orderID) 
        {
			try
            {
                var userData = User.GetUserData();
                int EmployeeID = int.Parse(userData.UserId);
                var result = OrderDataService.AcceptOrder(orderID , EmployeeID);
                if (result)
                    return Json(new { success = true, message = "Đã duyệt đơn hàng !" , orderId = orderID});
                else
                    return Json(new { success = false, message = "Không tìm thấy Đơn hàng !" + result + " " + orderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi duyệt đơn hàng!", error = ex.Message });
            }
        }

        public IActionResult Finish(int orderID)
        {
            try
            {
                var result = OrderDataService.FinishOrder(orderID);
                if (result)
                    return Json(new { success = true, message = "Đã xác nhận hoàn tất đơn hàng !", orderId = orderID });
                else
                    return Json(new { success = false, message = "Không tìm thấy Đơn hàng !" + result + " " + orderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xác nhận hoàn tất đơn hàng!", error = ex.Message });
            }
        }
        
        public IActionResult Cancel(int orderID)
        {
            try
            {
                var userData = User.GetUserData();
                int EmployeeID = int.Parse(userData.UserId);

                var result = OrderDataService.CancelOrder(orderID, EmployeeID);
                if (result)
                    return Json(new { success = true, message = "Hủy đơn hàng thành công !", orderId = orderID });
                else
                    return Json(new { success = false, message = "Không tìm thấy Đơn hàng ! " + result + " " + orderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi hủy đơn hàng!", error = ex.Message });
            }
        }
        
        public IActionResult Reject(int orderID)
        {
            try
            {
                var userData = User.GetUserData();
                int EmployeeID = int.Parse(userData.UserId);

                var result = OrderDataService.RejectOrder(orderID, EmployeeID);
                if (result)
                    return Json(new { success = true, message = "Từ chối đơn hàng thành công !", orderId = orderID });
                else
                    return Json(new { success = false, message = "Không tìm thấy Đơn hàng ! " + result + " " + orderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi từ chối đơn hàng!", error = ex.Message });
            }
        }

        public IActionResult Shipping(int id = 0)
        {
            if(id <= 0)
            {
                return Json(new { success = false, message = "Bạn chưa chọn nhân viên giao hàng"});
            }
            var data = OrderDataService.GetOrder(id);
            return View(data);
        }

        [HttpPost]
        public IActionResult OrderShipping(int orderID, int shipperID)
        {
            var result = OrderDataService.ShipOrder(orderID, shipperID);
            if (result)
                return Redirect("~/Order/Details/" + orderID);
            else
                return View("Index");
        }

        public IActionResult Delete(int orderID) 
        {
            try
            {
                var result = OrderDataService.DeleteOrder(orderID);
                if (result)
                    return Json(new { success = true, message = "Xóa đơn hàng thành công! ", orderId = orderID });
                else
                    return Json(new { success = false, message = "Không tìm thấy Đơn hàng! " + result + " " + orderID });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa đơn hàng! ", error = ex.Message });
            }
        }
    }


}
