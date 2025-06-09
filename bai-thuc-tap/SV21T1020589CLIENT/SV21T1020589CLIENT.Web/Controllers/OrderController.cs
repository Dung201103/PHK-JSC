using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020589CLIENT.BusinessLayers;
using SV21T1020589CLIENT.DomainModels;
using SV21T1020589CLIENT.Web.Models;

namespace SV21T1020589CLIENT.Web.Controllers
{
	[Authorize]
	public class OrderController : Controller
    {
		private const string SHOPPING_CART = "ShoppingCart";

		public IActionResult Index()
        {
            return View();
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
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (item.SalePrice < 0 || (item.Quantity < 0 && (item.Quantity + existsProduct.Quantity) < 1))
				return Json("Giá bán và số lượng không hợp lệ");

			if (existsProduct == null)
			{
				shoppingCart.Add(item);
			}
			else
			{
				existsProduct.Quantity += item.Quantity;
            }
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
			return Json("");
			//return Json(item.ProductID + item.ProductName);
			//return RedirectToAction("Index");
		}

		public IActionResult RemoveFromCart(int id = 0)
		{
			var shoppingCart = GetShoppingCart();
			int index = shoppingCart.FindIndex(m => m.ProductID == id);
			if (index > 0)
			{
				shoppingCart.RemoveAt(index);
			}
			ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            //return Json("");
            return RedirectToAction("Index");
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
			
			//int employeeID = 1;  Đây là phần của khác hàng nên k có employeeID , khi nào bên admin duyệt mới thêm EmployeeID vào

			List<OrderDetail> orderDetails = new List<OrderDetail>();
			foreach (var item in shoppingCart)
			{
				orderDetails.Add(new OrderDetail
				{
					ProductID = item.ProductID,
					Quantity = item.Quantity,
					SalePrice = item.SalePrice
				});
			}
			int orderID = OrderDataService.InitOrder(customerID, deliveryProvince, deliveryAddress, orderDetails);
			ClearCart();
			return Json(orderID);
		}

		public IActionResult History() 
		{
            return View();
		}
        // End Cart Function
        public IActionResult Cancel(int orderID)
        {
            try
            {
                var result = OrderDataService.CancelOrder(orderID);
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
    }
}
