using Dapper;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
	{
		public ShipperDAL(string connectionString) : base(connectionString)
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
                    FROM Shippers
                    WHERE (ShipperName LIKE @searchValue)
                ";
				var parameters = new { searchValue };
				count = connection.ExecuteScalar<int>(sql: sql, param: parameters);
				connection.Close();
			}
			return count;
		}

		public Shipper? Get(int id)
		{
			Shipper? data = null;
			using (var connection = OpenConnection())
			{
				var sql = @"SELECT * FROM Shippers WHERE ShipperID = @ShipperID";
				var parameters = new { ShipperID = id };
				data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters);
				connection.Close();
			}
			return data;
		}

		public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
		{
			List<Shipper> data = new List<Shipper>();
			searchValue = $"%{searchValue}%";
			using (var connection = OpenConnection())
			{
				var sql = @"
                    SELECT *
                    FROM Shippers
                    WHERE (ShipperName LIKE @searchValue)
                    ORDER BY ShipperName
                    LIMIT @pageSize OFFSET @offset
                ";

				if (pageSize == 0)
				{
					// Không phân trang
					sql = @"
                        SELECT *
                        FROM Shippers
                        WHERE (ShipperName LIKE @searchValue)
                        ORDER BY ShipperName
                    ";
				}

				var parameters = new
				{
					searchValue,
					pageSize,
					offset = (page - 1) * pageSize
				};

				data = connection.Query<Shipper>(sql: sql, param: parameters).ToList();
				connection.Close();
			}
			return data;
		}
	}
}
