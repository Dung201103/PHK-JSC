using Dapper;
using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue} %";
            using (var connection = OpenConnection())
            {
                var sql = @"
                    select count(*)
                    from Suppliers
                    where (SupplierName like @searchValue) or (ContactName like @searchValue)
                ";
                var parameters = new
                {
                    searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
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
                var sql = @"
                    select *
                    from (
                        select *,
                            ROW_NUMBER() over(order by SupplierName) as RowNumber
                        from Suppliers
                        where (SupplierName like @searchValue) or (ContactName like @searchValue)
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
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

    }
}
