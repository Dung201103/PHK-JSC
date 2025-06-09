using SV21T1020589CLIENT.DataLayers;
using SV21T1020589CLIENT.DataLayers.SQLServer;
using SV21T1020589CLIENT.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020589CLIENT.BusinessLayers
{
	public static class CustomerDataService
	{
		private static readonly ICustomerDAL customerDB;

		static CustomerDataService()
		{
			string connectionString = @"Server=DESKTOP-IT9GPIL;user id=sa;password=12345678;Database=LiteComerceDB;TrustServerCertificate=True";
			customerDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }
        
		/// <summary>
		/// Xác minh 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static Customer? Authorize(string username, string password)
        {
             return customerDB.Authorize(username, password);
        }
		
		/// <summary>
		/// Đổi mật khẩu
		/// </summary>
		/// <param name="username"></param>
		/// <param name="oldPassword"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
        public static bool ChangePassword(string CustomerID, string oldPassword, string newPassword)
        {
            bool result = customerDB.ChangePassword(CustomerID, oldPassword, newPassword);
            return result;
        }

        /// <summary>
        /// Lấy ra thông tin của 1 khách hàng nào đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
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
	}
}
