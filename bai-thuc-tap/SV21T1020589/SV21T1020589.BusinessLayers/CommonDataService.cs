using Azure;
using SV21T1020589.DataLayers;
using SV21T1020589.DataLayers.SQLServer;
using SV21T1020589.DomainModels;

namespace SV21T1020589.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;

        static CommonDataService()
        {
			//string connectionString = @"server=ADMIN-PC\MSSQLSERVER01;user=sa;password=123456;database=LiteCommerceDB_2023;TrustServerCertificate=true";
			//customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
			//shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
			//supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
			//categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
			//employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
			//provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);

			string connectionString = @"Server=DESKTOP-IT9GPIL;user id=sa;password=12345678;Database=LiteComerceDB;TrustServerCertificate=True";
			provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);

        }
        // Customers
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiễn thị trên mỗi trang</param>
        /// <param name="searchValue">Tên khác hàng hoặc tiên giao dich càn tìm</param>
        /// <returns></returns>
        public static List<Customer> ListOffCustomers(out int rowCount , int page = 1 , int pageSize = 0 , string searchValue = "") 
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page , pageSize , searchValue);
        }
        public static List<Customer> ListOffCustomers()
        {
            return customerDB.List();
        }
        /// <summary>
        /// Lấy ra thông tin của 1 khách hàng nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer?GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Cap nhat thong tin cua 1 khac hang nao do
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 khách hàng ...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id) {
            if (customerDB.InUsed(id)) {
                return false;
            }
            return customerDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra một khách hàng hiện đang có đơn hàng hay không >?????
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id) { 
            return customerDB.InUsed(id);
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
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Cap nhat thong tin cua 1 Người giao hàng nao do
        /// </summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 Người giao hàng ...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (customerDB.InUsed(id))
            {
                return false;
            }
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra một Người giao hàng hiện đang có đơn hàng hay không >?????
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }


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
        /// <summary>
        /// Bổ sung 1 Nhà cung cấp mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// Cap nhat thong tin cua 1 Nhà cung cấp nao do
        /// </summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 Nhà cung cấp ...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.InUsed(id))
            {
                return false;
            }
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra một Nhà cung cấp hiện đang có đơn hàng hay không >?????
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
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
        public static List<Category> ListOffCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy ra thông tin của 1 Mac hang nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Category> ListOffCategories()
        {
            return categoryDB.List();
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 Mac hang mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Cap nhat thong tin cua 1 Mac hang nao do
        /// </summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 Mac hang...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.InUsed(id))
            {
                return false;
            }
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra một Mac hang hiện đang có đơn hàng hay không >?????
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }


        // Employee
        /// <summary>
        /// Tìm kiếm và lấy danh sách Nhân viên dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiễn thị trên mỗi trang</param>
        /// <param name="searchValue">Tên loại hàng hoặc mô tả càn tìm</param>
        /// <returns></returns>
        public static List<Employee> ListOffEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }

        public static List<Employee> ListOffEmployees()
        {
            return employeeDB.List();
        }

        /// <summary>
        /// Lấy ra thông tin của 1 Nhân viên nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>       
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 Nhân viên mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// Cap nhat thong tin cua 1Nhân viên nao do
        /// </summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 Nhân viên...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.InUsed(id))
            {
                return false;
            }
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra một Nhân viên hiện đang có đơn hàng hay không >?????
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }


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
