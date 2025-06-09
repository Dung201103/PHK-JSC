using Dapper;
using SV21T1020589CLIENT.DomainModels;
using System.Diagnostics;

namespace SV21T1020589CLIENT.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

		public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"
                    select count(*)
                    from Products
                    where (ProductName like @searchValue) 
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

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }


        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT *
                    FROM (
                        SELECT *,
                            ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                        FROM Products
                        WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                            AND (@categoryID = 0 OR CategoryID = @categoryID)
                            AND (@supplierID = 0 OR SupplierId = @supplierID)
                            AND (Price >= @minPrice)
                            AND (@maxPrice <= 0 OR Price <= @maxPrice)
                            AND IsSelling = 1
                        ) AS t
                    WHERE (@pageSize = 0)
                        OR (RowNumber BETWEEN (@page - 1)*@pageSize + 1 AND @page * @pageSize)
                ";
                var parameters = new
                {
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice,
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            IList<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder
                FROM ProductAttributes
                WHERE ProductID = @productID
                ORDER BY DisplayOrder";
                var parameters = new
                {
                    productID = productID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            IList<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection()) {
                var sql = @"SELECT PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden
                            FROM ProductPhotos
                            WHERE ProductID = @productID AND IsHidden = 0
                            ORDER BY DisplayOrder";
                var parameters = new
                {
                    productID = productID
                };
                data = connection.Query<ProductPhoto>(sql :  sql ,param : parameters, commandType : System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

    }
}
