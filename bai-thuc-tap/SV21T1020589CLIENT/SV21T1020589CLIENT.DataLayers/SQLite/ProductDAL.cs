using Dapper;
using SV21T1020589CLIENT.DomainModels;
using System.Diagnostics;

namespace SV21T1020589CLIENT.DataLayers.SQLite
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
                    SELECT COUNT(*)
                    FROM Products
                    WHERE (@searchValue = '' OR ProductName LIKE @searchValue)
                        AND (@categoryID = 0 OR CategoryID = @categoryID)
                        AND (@supplierID = 0 OR SupplierID = @supplierID)
                        AND (Price >= @minPrice)
                        AND (@maxPrice <= 0 OR Price <= @maxPrice)
                        AND IsSelling = 1
                ";
				var parameters = new { searchValue, categoryID, supplierID, minPrice, maxPrice };
				count = connection.ExecuteScalar<int>(sql, parameters);
			}
			return count;
		}

		public Product? Get(int productID)
		{
			using var connection = OpenConnection();
			var sql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
			return connection.QueryFirstOrDefault<Product>(sql, new { ProductID = productID });
		}

		public ProductAttribute? GetAttribute(long attributeID)
		{
			using var connection = OpenConnection();
			var sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
			return connection.QueryFirstOrDefault<ProductAttribute>(sql, new { AttributeID = attributeID });
		}

		public ProductPhoto? GetPhoto(long photoID)
		{
			using var connection = OpenConnection();
			var sql = @"SELECT * FROM ProductPhotos WHERE PhotoID = @PhotoID";
			return connection.QueryFirstOrDefault<ProductPhoto>(sql, new { PhotoID = photoID });
		}

		public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
		{
			List<Product> data = new();
			searchValue = $"%{searchValue}%";
			using var connection = OpenConnection();

			var offset = (page - 1) * pageSize;

			var sql = @"
                SELECT *
                FROM Products
                WHERE (@searchValue = '' OR ProductName LIKE @searchValue)
                    AND (@categoryID = 0 OR CategoryID = @categoryID)
                    AND (@supplierID = 0 OR SupplierID = @supplierID)
                    AND (Price >= @minPrice)
                    AND (@maxPrice <= 0 OR Price <= @maxPrice)
                    AND IsSelling = 1
                ORDER BY ProductName
                LIMIT @pageSize OFFSET @offset
            ";

			if (pageSize == 0)
			{
				sql = @"
                    SELECT *
                    FROM Products
                    WHERE (@searchValue = '' OR ProductName LIKE @searchValue)
                        AND (@categoryID = 0 OR CategoryID = @categoryID)
                        AND (@supplierID = 0 OR SupplierID = @supplierID)
                        AND (Price >= @minPrice)
                        AND (@maxPrice <= 0 OR Price <= @maxPrice)
                        AND IsSelling = 1
                    ORDER BY ProductName
                ";
			}

			var parameters = new
			{
				searchValue,
				categoryID,
				supplierID,
				minPrice,
				maxPrice,
				pageSize,
				offset
			};

			data = connection.Query<Product>(sql, parameters).ToList();
			return data;
		}

		public IList<ProductAttribute> ListAttributes(int productID)
		{
			using var connection = OpenConnection();
			var sql = @"
                SELECT AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder
                FROM ProductAttributes
                WHERE ProductID = @productID
                ORDER BY DisplayOrder";
			return connection.Query<ProductAttribute>(sql, new { productID }).ToList();
		}

		public IList<ProductPhoto> ListPhotos(int productID)
		{
			using var connection = OpenConnection();
			var sql = @"
                SELECT PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden
                FROM ProductPhotos
                WHERE ProductID = @productID AND IsHidden = 0
                ORDER BY DisplayOrder";
			return connection.Query<ProductPhoto>(sql, new { productID }).ToList();
		}
	}
}
