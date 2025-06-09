using SV21T1020589CLIENT.DomainModels;

namespace SV21T1020589CLIENT.DataLayers
{
	public interface ICustomerDAL
	{
		/// <summary>
		/// lấy 1 bản ghi dữ liệu của kiểu T dựa vào khóa chính / ID (trả về null nếu dữ liệu k tồn tại)
		/// </summary>
		/// <param name="id"> </param>
		/// <returns></returns>
		Customer? Get(int id);

		Customer? Authorize(string email, string password);

		/// <summary>
		/// Bổ sung 1 bản ghi vào CSDL, hàm trả về ID của dữ liệu vừa bổ sung nếu có
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		int Add(Customer data);

		/// <summary>
		/// Cập nhật 1 bản ghi dữ liệu
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Update(Customer data);

        /// <summary>
        /// Doi mat khau
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string CustomerID, string oldPassword, string newPassword);
    }
}



		
