using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020589CLIENT.Web
{
    /// <summary>
    /// Lưu giữ thông tin của người dùng được ghi trong Cookie
    /// </summary>
    public class WebUserData
    {
        public string CustomerID { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;


        //public string UserId { get; set; } = "";
        //public string UserName { get; set; } = "";
        //public string DisplayName { get; set; } = "";
        //public string Photo { get; set; } = "";
        //public List<string>? Roles { get; set; }

        /// <summary>
        /// Danh sách các Claims 
        /// </summary>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(CustomerID),CustomerID),
                    new Claim(nameof(CustomerName),CustomerName),
                    new Claim(nameof(ContactName),ContactName),
                    new Claim(nameof(Province),Province),
                    new Claim(nameof(Address),Address),
                    new Claim(nameof(Phone),Phone)
                    //new Claim(nameof(Photo),Photo)
                };
                return claims;
            }
        }

        /// <summary>
        /// Tạo ClaimsPrincipal dựa trên thông tin của người dùng (cần lưu trong Cookiie)
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            var identity = new ClaimsIdentity(Claims , CookieAuthenticationDefaults.AuthenticationScheme); // thông tin định danh của người dùng
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
