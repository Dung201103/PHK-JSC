using System.Security.Claims;

namespace SV21T1020589CLIENT.Web
{
    /// <summary>
    /// Tạo thêm phương thức (hàm) mở rộng cho Pricipal dể lấy thông tin của người dùng
    /// dự trên Cookie
    /// </summary>
    public static class WebUserExtensions
    {
        public static WebUserData? GetUserData(this ClaimsPrincipal principal) 
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                {
                    return null;
                }
                var userData = new WebUserData();
                userData.CustomerID = principal.FindFirstValue(nameof(userData.CustomerID)) ?? "";
                userData.CustomerName = principal.FindFirstValue(nameof(userData.CustomerName)) ?? "";
                userData.ContactName = principal.FindFirstValue(nameof(userData.ContactName)) ?? "";
                userData.Province = principal.FindFirstValue(nameof(userData.Province)) ?? "";
                userData.Phone = principal.FindFirstValue(nameof(userData.Phone)) ?? "";
                userData.Address = principal.FindFirstValue(nameof(userData.Address)) ?? "";

                return userData;
            }
            catch
            {
                return null;
            }
        }
    }
}
