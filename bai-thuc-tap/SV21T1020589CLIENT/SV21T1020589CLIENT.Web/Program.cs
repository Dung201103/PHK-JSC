using Microsoft.AspNetCore.Authentication.Cookies;

namespace SV21T1020589CLIENT.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";        //Tên Cookie
                                option.LoginPath = "/Customer/Login";                //URL trang đăng nhập 
                                option.AccessDeniedPath = "/Customer/AccessDenined"; //URL trang cấm truy cập
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);     //Thời gian tồn tại của Cookie
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline. : Trình tự xử lý khi người dùng gửi yêu cầu lên server
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();   //Xác thực

            app.UseAuthorization();    //Phân quyền

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            ApplicationContext.Configure(
                context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
             );

            //Khởi tạo cấu hình cho BusinessLayer
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020589CLIENT") ?? "DefaultConnectionString";
            SV21T1020589CLIENT.BusinessLayers.Configuration.Initialize(connectionString);

            app.Run();
        }
    }
}
