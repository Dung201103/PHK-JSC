using SV21T1020589CLIENT.BusinessLayers;
using SV21T1020589CLIENT.DataLayers.SQLite;
using SV21T1020589CLIENT.DataLayers;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ISimpleQueryDAL<Category> categoryDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;

        static CommonDataService()
        {

			string connectionString = @"Data Source=D:\Lite.db;";
			provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);

        }

        

        // Shippers
        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiễn thị trên mỗi trang</param>
        /// <param name="searchValue">Tên người giao hàng hoặc sdt càn tìm</param>
        /// <returns></returns>
        public static List<Shipper> ListOffShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        public static List<Shipper> ListOffShippers()
        {
            return shipperDB.List();
        }
        /// <summary>
        /// Lấy ra thông tin của 1 Người giao hàng nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 Người giao hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
       


        // Suppliers
        /// <summary>
        /// Tìm kiếm và lấy danh sách Nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiễn thị trên mỗi trang</param>
        /// <param name="searchValue">Tên nhà cung cấp hoặc tiên giao dich càn tìm</param>
        /// <returns></returns>
        public static List<Supplier> ListOffSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy ra thông tin của 1 Nhà cung cấp nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Supplier> ListOffSuppliers()
        {
            return supplierDB.List();
        }
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
      


        // Category
        /// <summary>
        /// Tìm kiếm và lấy danh sách Loại hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiễn thị trên mỗi trang</param>
        /// <param name="searchValue">Tên loại hàng hoặc mô tả càn tìm</param>
        /// <returns></returns>
        public static List<Category> ListOffCategories()
        {
			return categoryDB.List();
		}
        /////// <summary>
        /////// Lấy ra thông tin của 1 Mac hang nào đó
        /////// </summary>
        /////// <param name="id"></param>
        /////// <returns></returns>
        ////public static List<Category> ListOffCategories()
        ////{
        ////    return categoryDB.List();
        ////}
        //public static Category? GetCategory(int id)
        //{
        //    return categoryDB.Get(id);
        //}
     

       
        // Provinces
        /// <summary>
        /// Danh sách các tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOffProvinces()
        {
            return provinceDB.List();
        }
    }
}
