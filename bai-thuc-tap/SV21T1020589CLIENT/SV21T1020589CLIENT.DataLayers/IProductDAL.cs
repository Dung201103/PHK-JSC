using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers
{
    public interface IProductDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng nhị phân
        /// </summary>
        /// <param name="page"> Trang cần hiển thị </param>
        /// <param name="pageSize"> Số dòng trên mỗi trang (0 nếu không phân trang) </param>
        /// <param name="searchValue"> Tên mặt hàng cần tìm (chuỗi rỗng nếu k tìm kiếm) </param>
        /// <param name="categoryID"> Mã loại hàng cần tìm kiếm (0 nếu không tìm theo loại hàng ) </param>
        /// <param name="supplierID"> Mã nhà cung cấp cần tìm kiếm (0 nếu không tìm theo nhà cung cấp) </param>
        /// <param name="minPrice"> Mức giá nhỏ nhất trong khoảng giá cần tìm </param>
        /// <param name="maxPrice"> Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá tìm kiếm lớn nhất)</param>
        /// <returns></returns>
        List<Product> List(int page =1 , int pageSize = 0 , 
           string searchValue ="" , int categoryID = 0 , int supplierID = 0 ,
           decimal minPrice = 0 , decimal maxPrice = 0);

        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue"> Tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm) </param>
        /// <param name="categoryID"> Mã loại hàng cần tìm (0 nếu không tìm theo loại hàng) </param>
        /// <param name="supplierID"> Mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp) </param>
        /// <param name="minPrice"> Mức giá nhỏ nhất trong khoảng giá cần tìm </param>
        /// <param name="maxPrice"> Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá tìm kiếm lớn nhất) </param>
        /// <returns></returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
       
        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Product? Get(int productID);

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhotos(int productID);

        /// <summary>
        /// Lấy thông tin ảnh dựa theo ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        ProductPhoto? GetPhoto(long photoID);


        /// <summary>
        /// Lấy danh sách thuộc tính của mặt hàng  (sắp xếp theo thứ tự của DisplayOrder)   
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductAttribute> ListAttributes(int productID);

        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttribute(long attributeID);

    }
}
