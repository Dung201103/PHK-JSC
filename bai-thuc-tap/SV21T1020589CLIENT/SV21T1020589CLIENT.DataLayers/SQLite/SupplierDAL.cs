using Dapper;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
	{
		public SupplierDAL(string connectionString) : base(connectionString)
		{
		}

		public int Count(string searchValue = "")
		{
			int count = 0;
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				var sql = @"
                    SELECT COUNT(*)
                    FROM Suppliers
                    WHERE (SupplierName LIKE @searchValue) OR (ContactName LIKE @searchValue)
                ";
				var parameters = new { searchValue };
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters);
				connection.Close();
			}
			return count;
		}

		public Supplier? Get(int id)
		{
			Supplier? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
				var parameters = new { SupplierID = id };
				data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters);
				connection.Close();
			}
			return data;
		}

		public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Supplier> data = new List<Supplier>();
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				string sql;

				if (pageSize == 0)
				{
					sql = @"
                        SELECT *
                        FROM Suppliers
                        WHERE (SupplierName LIKE @searchValue) OR (ContactName LIKE @searchValue)
                        ORDER BY SupplierName
                    ";
				}
				else
				{
					sql = @"
                        SELECT *
                        FROM Suppliers
                        WHERE (SupplierName LIKE @searchValue) OR (ContactName LIKE @searchValue)
                        ORDER BY SupplierName
                        LIMIT @pageSize OFFSET @offset
                    ";
				}

				var parameters = new
				{
					searchValue,
					pageSize,
					offset = (page - 1) * pageSize
				};

				data = connection.Query<Supplier>(sql: sql, param: parameters).ToList();
				connection.Close();
			}
			return data;
		}
	}
}
