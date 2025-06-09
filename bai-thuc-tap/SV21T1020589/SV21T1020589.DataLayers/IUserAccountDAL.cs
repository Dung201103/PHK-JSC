using SV21T1020589.DomainModels;

namespace SV21T1020589.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Xac thuc danh tinh
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username , string password);

        /// <summary>
        /// Doi mat khau
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string username , string oldPassword, string newPassword);
    }
}
