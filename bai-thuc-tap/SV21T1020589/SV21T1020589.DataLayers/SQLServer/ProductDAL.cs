using Dapper;
using SV21T1020589.DomainModels;
using System.Diagnostics;

namespace SV21T1020589.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName, ProductDescription, CategoryID, SupplierID, Unit , Price, Photo, IsSelling)
                                    values(@ProductName, @ProductDescription, @CategoryID, @SupplierID, @Unit, @Price, @Photo, @IsSelling);

                                    select @@identity;
                                end
                ";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription,
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                //thuc thi cau lenh SQL
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductAttributes where AttributeName = @AttributeName)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes (ProductID, AttributeName ,AttributeValue , DisplayOrder)
                                    values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);

                                    select @@identity;
                                end
                ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder
                };
                //thuc thi cau lenh SQL
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductPhotos where [Description] = @Description)
                                select -1
                            else
                                begin
                                    insert into ProductPhotos(ProductID, Photo, [Description] , DisplayOrder, IsHidden)
                                    values(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);

                                    select @@identity;
                                end
                ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                //thuc thi cau lenh SQL
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
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

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where ProductID = @ProductID
                            delete from ProductAttributes where ProductID = @ProductID
                            delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();

            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();

            }
            return result;
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

        public bool IsUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from OrderDetails where ProductID = @ProductID)
                                select 1 
                            else 
                                select 0"; 
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
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

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                           if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                   update Products
                                   set 
                                        ProductName = @ProductName , 
                                        ProductDescription = @ProductDescription ,  
                                        CategoryID = @CategoryID, 
                                        SupplierID = @SupplierID,
                                        Unit = @Unit,
                                        Price = @Price,   
                                        Photo = @Photo, 
                                        IsSelling = @IsSelling
                                where ProductID = @ProductID
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                           if not exists(select * from ProductAttributes where AttributeID <> @AttributeID  and AttributeName = @AttributeName)
                                begin
                                   update ProductAttributes
                                   set 
                                        ProductID = @ProductID , 
                                        AttributeName = @AttributeName ,  
                                        AttributeValue = @AttributeValue, 
                                        DisplayOrder = @DisplayOrder
                                     
                                where AttributeID = @AttributeID
                                end";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                           if not exists(select * from ProductPhotos where PhotoID <> @PhotoID and [Description] = @Description)
                                begin
                                   update ProductPhotos
                                   set 
                                        ProductID = @ProductID , 
                                        Photo = @Photo ,  
                                        [Description] = @Description, 
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                     
                                where PhotoID = @PhotoID
                                end";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
