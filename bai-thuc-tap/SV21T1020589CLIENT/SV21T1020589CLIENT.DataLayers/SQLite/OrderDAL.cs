using Dapper;
using SV21T1020589CLIENT.DomainModels;
using Microsoft.Data.Sqlite; // SQLite
namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class OrderDAL : BaseDAL, IOrderDAL
	{
		public OrderDAL(string connectionString) : base(connectionString) { }

		public int Add(Order data)
		{
			int id = 0;
			using (var connection = OpenConnection())
			{
				var sql = @"INSERT INTO Orders (CustomerID, OrderTime, DeliveryProvince, DeliveryAddress, Status)
                            VALUES (@CustomerID, CURRENT_TIMESTAMP, @DeliveryProvince, @DeliveryAddress, @Status);
                            SELECT last_insert_rowid();";

				var parameters = new
				{
					CustomerID = data.CustomerID,
					DeliveryProvince = data.DeliveryProvince,
					DeliveryAddress = data.DeliveryAddress,
					Status = data.Status
				};
				id = connection.ExecuteScalar<int>(sql, parameters);
			}
			return id;
		}

		public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
		{
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = OpenConnection())
			{
				var sql = @"SELECT COUNT(*) FROM Orders AS o
                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                            WHERE (@Status = 0 OR o.Status = @Status)
                                AND (@FromTime IS NULL OR o.OrderTime >= @FromTime)
                                AND (@ToTime IS NULL OR o.OrderTime <= @ToTime)
                                AND (@SearchValue = '' OR c.CustomerName LIKE @SearchValue OR e.FullName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)";

				var parameters = new { status, fromTime, toTime, searchValue };
				return connection.ExecuteScalar<int>(sql, parameters);
			}
		}

		public bool Delete(int data)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID;
                            DELETE FROM Orders WHERE OrderID = @OrderID;";
				return connection.Execute(sql, new { OrderID = data }) > 0;
			}
		}

		public bool DeleteDetail(int orderID, int productID)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID;";
				return connection.Execute(sql, new { OrderID = orderID, ProductID = productID }) > 0;
			}
		}

		public Order? Get(int orderID)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT o.*, c.CustomerName, c.ContactName AS CustomerContactName,
                                   c.Address AS CustomerAddress, c.Phone AS CustomerPhone, c.Email AS CustomerEmail,
                                   e.FullName AS EmployeeName, s.ShipperName, s.Phone AS ShipperPhone
                            FROM Orders o
                            LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                            WHERE o.OrderID = @OrderID;";
				return connection.QueryFirstOrDefault<Order>(sql, new { OrderID = orderID });
			}
		}

		public OrderDetail? GetDetail(int orderID, int productID)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT od.*, p.ProductName, p.Photo, p.Unit
                            FROM OrderDetails od
                            JOIN Products p ON od.ProductID = p.ProductID
                            WHERE od.OrderID = @OrderID AND od.ProductID = @ProductID;";
				return connection.QueryFirstOrDefault<OrderDetail>(sql, new { OrderID = orderID, ProductID = productID });
			}
		}

		public IList<Order> List(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
		{
			if (!string.IsNullOrEmpty(searchValue))
				searchValue = "%" + searchValue + "%";

			using (var connection = OpenConnection())
			{
				var offset = (page - 1) * pageSize;

				var sql = @$"SELECT o.*, c.CustomerName, c.ContactName AS CustomerContactName,
                                    c.Address AS CustomerAddress, c.Phone AS CustomerPhone, c.Email AS CustomerEmail,
                                    e.FullName AS EmployeeName, s.ShipperName, s.Phone AS ShipperPhone
                            FROM Orders o
                            LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                            WHERE (@Status = 0 OR o.Status = @Status)
                                AND (@FromTime IS NULL OR o.OrderTime >= @FromTime)
                                AND (@ToTime IS NULL OR o.OrderTime <= @ToTime)
                                AND (@SearchValue = '' OR c.CustomerName LIKE @SearchValue OR e.FullName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)
                            ORDER BY o.OrderTime DESC
                            {(pageSize > 0 ? "LIMIT @PageSize OFFSET @Offset" : "")};";

				return connection.Query<Order>(sql, new
				{
					status,
					fromTime,
					toTime,
					searchValue,
					PageSize = pageSize,
					Offset = offset
				}).ToList();
			}
		}

		public IList<Order> ListOrder(int CustomerID)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT 
                                o.OrderID,
                                c.CustomerName,
                                o.OrderTime,
                                o.FinishedTime,
                                o.Status,
                                SUM(od.Quantity * od.SalePrice) AS TotalPrice
                            FROM Orders o
                            JOIN Customers c ON o.CustomerID = c.CustomerID
                            JOIN OrderDetails od ON o.OrderID = od.OrderID
                            WHERE o.CustomerID = @CustomerID
                            GROUP BY o.OrderID, c.CustomerName, o.OrderTime, o.FinishedTime, o.Status
                            ORDER BY o.OrderTime DESC;";
				return connection.Query<Order>(sql, new { CustomerID }).ToList();
			}
		}

		public IList<OrderDetail> ListDetails(int orderID)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT od.*, p.ProductName, p.Photo, p.Unit
                            FROM OrderDetails od
                            JOIN Products p ON od.ProductID = p.ProductID
                            WHERE od.OrderID = @OrderID;";
				return connection.Query<OrderDetail>(sql, new { OrderID = orderID }).ToList();
			}
		}

		public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                            VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)
                            ON CONFLICT(OrderID, ProductID) DO UPDATE SET
                                Quantity = excluded.Quantity,
                                SalePrice = excluded.SalePrice;";
				return connection.Execute(sql, new { orderID, productID, quantity, salePrice }) > 0;
			}
		}

		public bool Update(Order data)
		{
			using (var connection = OpenConnection())
			{
				var sql = @"UPDATE Orders
                            SET CustomerID = @CustomerID,
                                OrderTime = @OrderTime,
                                DeliveryProvince = @DeliveryProvince,
                                DeliveryAddress = @DeliveryAddress,
                                EmployeeID = @EmployeeID,
                                AcceptTime = @AcceptTime,
                                ShipperID = @ShipperID,
                                ShippedTime = @ShippedTime,
                                FinishedTime = @FinishedTime,
                                Status = @Status
                            WHERE OrderID = @OrderID;";
				return connection.Execute(sql, data) > 0;
			}
		}
	}
}
