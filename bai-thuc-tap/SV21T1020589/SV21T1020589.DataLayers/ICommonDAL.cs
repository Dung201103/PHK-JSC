namespace SV21T1020589.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu chung thường dùng trên bảng
    /// </summary>
    /// <typeparam name="T"></typeparam> T là 1 lớp dữ liệu bất kỳ ta cần xử lý (Customer ,Employees , Shippers ,...)
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu là T , dưới dạng có phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị thứ mấy (ở phần phân trang) </param>
        /// <param name="pageSize">Số dòng được hiển thị trên 1 trang (= 0  nếu k phân trang)</param>
        /// <param name="searchValue">Giá trị chúng ta cần tìm kiếm (chuỗi rỗng nếu lấy toàn bộ dữ liệu) </param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");

        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Giá trị cần tìm kiếm (chuỗi rỗng nếu dếm trên toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");
        /// <summary>
        /// lấy 1 bản ghi dữ liệu của kiểu T dựa vào khóa chính / ID (trả về null nếu dữ liệu k tồn tại)
        /// </summary>
        /// <param name="id"> </param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Bổ sung 1 bản ghi vào CSDL, hàm trả về ID của dữ liệu vừa bổ sung nếu có
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cập nhật 1 bản ghi dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);  
        /// <summary>
        /// Xóa 1 bản ghi dữ liệu dựa vào khóa chính / ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem có dữ liệu đang tham chiếu ở bảng khác hay k
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}

//CRUD 
//C : create (insert)
//R : read (select)
//U : update
//D : delete

//ctr + m+ o
//ctr m l