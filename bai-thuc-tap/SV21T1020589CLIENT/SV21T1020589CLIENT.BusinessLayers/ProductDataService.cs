
using SV21T1020589CLIENT.BusinessLayers;
using SV21T1020589CLIENT.DataLayers.SQLServer;
using SV21T1020589CLIENT.DataLayers;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;


        static ProductDataService()
        {
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
            return productDB.List(1, 0, searchValue);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "",
            int cateogryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue);
            return productDB.List(page, pageSize, searchValue, cateogryId, supplierId, minPrice, maxPrice);
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
        public static ProductPhoto? GetPhoto(long productID)
        {
            return productDB.GetPhoto(productID);
        }
     
        // ProductAttribute
        public static IList<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID);
        }
        public static ProductAttribute? GetAttribute(long productID)
        {
            return productDB.GetAttribute(productID);
        }
 

    }
}
