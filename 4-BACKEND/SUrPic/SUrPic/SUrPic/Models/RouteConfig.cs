namespace SUrPic.Models
{
    public class RouteConfig
    {
        public static void RegisterRoutes(WebApplication app)
        {
            //Home
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //Đăng nhập
            app.MapControllerRoute(
                name: "login",
                pattern: "/login",
                defaults: new { controller = "Account", action = "Login" }
                );

            //Đăng ký
            app.MapControllerRoute(
                name: "register",
                pattern: "/register",
                defaults: new { controller = "Account", action = "Register" }
                );

            //Trang cá nhân
            app.MapControllerRoute(
                name: "profile",
                pattern: "/profile/{username?}",
                defaults: new { controller = "Account", action = "Profile" }
                );

            //Quan tâm
            app.MapControllerRoute(
                name: "follow",
                pattern: "/follow",
                defaults: new { controller = "Account", action = "Follow" }
                );


            //Tìm kiếm
            app.MapControllerRoute(
                name: "search",
                pattern: "/search",
                defaults: new { controller = "Home", action = "Search" }
                );

            //Tìm kiếm TK
            app.MapControllerRoute(
                name: "SearchAcc",
                pattern: "/search/acc/{keyword?}",
                defaults: new { controller = "Account", action = "SearchAcc" }
                );

            //Tìm kiếm HA
            app.MapControllerRoute(
                name: "SearchPic",
                pattern: "/search/pic/{keyword?}",
                defaults: new { controller = "Picture", action = "SearchPic" }
                );

            //Tải ảnh lên
            app.MapControllerRoute(
                name: "Upload",
                pattern: "/upload",
                defaults: new { controller = "Picture", action = "Upload" }
                );

            //Xem ảnh
            app.MapControllerRoute(
                name: "ViewPic",
                pattern: "/picture/{id?}",
                defaults: new { controller = "Picture", action = "ViewPic" }
                );

        }
    }
}
