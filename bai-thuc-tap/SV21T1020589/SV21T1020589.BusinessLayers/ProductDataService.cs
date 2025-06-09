using SV21T1020589.DataLayers;
using SV21T1020589.DataLayers.SQLServer;
using SV21T1020589.DomainModels;

namespace SV21T1020589.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        
        static ProductDataService() {
			string connectionString = @"Server=DESKTOP-IT9GPIL;user id=sa;password=12345678;Database=LiteComerceDB;TrustServerCertificate=True";
			productDB = new ProductDAL(connectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng k phân trang
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(1,0,searchValue);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "" , 
            int cateogryId = 0 , int supplierId = 0 , decimal minPrice = 0 , decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue);
            return productDB.List(page, pageSize, searchValue, cateogryId , supplierId , minPrice , maxPrice);
        }

        /// <summary>
        /// Lấy thông tin mặt hàng theo mã mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Thêm 1 sản phẩm mới theo Data nhập vào
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct (Product data)
        {
            return productDB.Add(data);
        }

        public static bool UpdateProduct(Product data) { 
            return productDB.Update(data);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID) {
            if (productDB.IsUsed(productID))
            {
                return false;
            }
            return productDB.Delete(productID);
        }
        /// <summary>
        /// Kiểm tra sự tồn tại của sản phẩm trong bản khác
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool IsUsedProduct(int productID) { 
            return productDB.IsUsed(productID);
        }

        // ProductPhotos
        /// <summary>
        /// Lay danh sach photo cua mat hang do
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static IList<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID);
        }
        public static ProductPhoto? GetPhoto(long productID) { 
            return productDB.GetPhoto(productID);
        }
        public static long AddPhoto(ProductPhoto data) { 
            return productDB.AddPhoto(data);
        }
        public static bool UpdatePhoto(ProductPhoto data) { 
            return productDB.UpdatePhoto(data);
        }
        public static bool DeletePhoto(long photoID) {
            return productDB.DeletePhoto(photoID);
        }

        // ProductAttribute
        public static IList<ProductAttribute> ListAttributes(int productID) { 
            return productDB.ListAttributes(productID);
        }
        public static ProductAttribute? GetAttribute(long productID)
        {
            return productDB.GetAttribute(productID);
        }
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        public static bool UpdateAttribute(ProductAttribute data) { 
            return productDB.UpdateAttribute(data);
        }
        public static bool DelAttribute(long attributeID) {
            return productDB.DeleteAttribute(attributeID);
        }

    }
}
