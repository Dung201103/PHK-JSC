using Dapper;
using Microsoft.Data.Sqlite;
using SV21T1020589CLIENT.DataLayers.SQLite;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLite
{
	public class CategoryDAL : BaseDAL, ISimpleQueryDAL<Category>
	{
		public CategoryDAL(string connectionString) : base(connectionString)
		{
		}

		public List<Category> List()
		{
			using (var connection = OpenConnection())
			{
				var sql = "SELECT * FROM Categories ORDER BY CategoryName";
				return connection.Query<Category>(sql).ToList();
			}
		}
	}
}


/*
    public CategoryDAL(string connectionString) : base(connectionString)
        {
        }
   

        public Category? Get(int id)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id
                };
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }


        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"
                    select *
                    from (
	                    select *,
		                    ROW_NUMBER() over(order by CategoryName) as RowNumber
	                    from Categories
	                    where (CategoryName like @searchValue) or (Description like @searchValue)
	                    ) as t
                    where (@pageSize = 0)
	                    or (t.RowNumber between (@page -1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber
                ";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

		public List<Category> List()
		{
			List<Category> data = new List<Category> ();
            using (var connection = OpenConnection()) {
                var sql = @"select * from Categories";
                data = connection.Query<Category>(sql : sql , commandType: System.Data.CommandType.Text).ToList ();
                connection.Close();
			}
            return data;
		}
 
 */