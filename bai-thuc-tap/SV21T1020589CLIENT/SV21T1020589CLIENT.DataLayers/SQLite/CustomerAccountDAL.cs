using Dapper;
using Microsoft.Data.Sqlite; // SQLite
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class CustomerAccountDAL : BaseDAL, ICustomerDAL
	{
		public CustomerAccountDAL(string connectionString) : base(connectionString)
		{
		}

		public int Add(Customer data)
		{
			using (var connection = OpenConnection())
			{
				// Kiểm tra trùng email
				var checkSql = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
				int exists = connection.ExecuteScalar<int>(checkSql, new { data.Email });
				if (exists > 0)
					return -1;

				// Thêm khách hàng
				var insertSql = @"INSERT INTO Customers 
                                    (CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                                  VALUES
                                    (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, @IsLocked);
                                  SELECT last_insert_rowid();";
				var parameters = new
				{
					CustomerName = data.CustomerName ?? "",
					ContactName = data.ContactName ?? "",
					Province = data.Province ?? "",
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
					Password = data.Password ?? "",
					IsLocked = data.IsLocked
				};

				int id = connection.ExecuteScalar<int>(insertSql, parameters);
				return id;
			}
		}

		public Customer? Authorize(string username, string password)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT * FROM Customers 
                            WHERE Email = @Email AND Password = @Password AND IsLocked = 0";
				return connection.QueryFirstOrDefault<Customer>(sql, new { Email = username, Password = password });
			}
		}

		public bool ChangePassword(string customerId, string oldPassword, string newPassword)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"UPDATE Customers 
                            SET Password = @newPassword 
                            WHERE CustomerID = @CustomerID AND Password = @oldPassword";
				var parameters = new
				{
					CustomerID = customerId,
					newPassword,
					oldPassword
				};
				return connection.Execute(sql, parameters) > 0;
			}
		}

		public Customer? Get(int id)
		{
			using (var connection = OpenConnection())
			{
				var sql = "SELECT * FROM Customers WHERE CustomerID = @CustomerID";
				return connection.QueryFirstOrDefault<Customer>(sql, new { CustomerID = id });
			}
		}

		public bool Update(Customer data)
		{
			using (var connection = OpenConnection())
			{
				// Kiểm tra email trùng
				var checkSql = @"SELECT COUNT(*) FROM Customers 
                                 WHERE CustomerID != @CustomerID AND Email = @Email";
				int exists = connection.ExecuteScalar<int>(checkSql, new { data.CustomerID, data.Email });
				if (exists > 0)
					return false;

				var sql = @"UPDATE Customers SET 
                                CustomerName = @CustomerName,
                                ContactName = @ContactName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                Password = @Password
                            WHERE CustomerID = @CustomerID";
				var parameters = new
				{
					CustomerID = data.CustomerID,
					CustomerName = data.CustomerName ?? "",
					ContactName = data.ContactName ?? "",
					Province = data.Province ?? "",
					Address = data.Address ?? "",
					Phone = data.Phone ?? "",
					Email = data.Email ?? "",
					Password = data.Password ?? ""
				};
				return connection.Execute(sql, parameters) > 0;
			}
		}
	}
}
