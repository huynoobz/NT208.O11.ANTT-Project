using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SUrPic.Models;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SUrPic.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        SqlDataReader? dataReader;

        public AccountController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        //Giao diện đăng nhập
        public IActionResult Login()
        {
            return View();
        }

        //Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await _contextAccessor.HttpContext.SignOutAsync();
            _contextAccessor.HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        //Giao diện đăng ký
        public IActionResult Register()
        {
            return View();
        }

        //Xác thực tài khoản
        [HttpPost]
        public async Task<IActionResult> Verify(Account account)
        {
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Băm mật khẩu
            string hashPass = SHA256.HashData(account.password.utf8Str2Bytes()).toBase64Str();

            //Câu lệnh SQL
            command.CommandText = "select * from ACCOUNT where USERNAME='" + account.userName + "' and HASH_PASSWORD='" + hashPass + "'";

            //Đọc kết quả truy vấn
            dataReader = command.ExecuteReader();

            //Xác thực tài khoản
            if (dataReader.Read())
            {
                connection.Close();
                var claims =new List<Claim> { new Claim(ClaimTypes.Name, account.userName) };
                var id = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                var prin=new ClaimsPrincipal(id);
                await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prin);
                _contextAccessor.HttpContext.Session.SetString("username", account.userName);

                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["ErrorMessage"] = "*Đăng nhập thất bại! Kiểm tra lại tên đăng nhập và mật khẩu của bạn!";
                connection.Close();
                return RedirectToAction("Login");
            }
        }

        //Đăng ký
        [HttpPost]
        public IActionResult AddAccount(Account account)
        {
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Băm mật khẩu
            string hashPass = SHA256.HashData(account.password.utf8Str2Bytes()).toBase64Str();

            //Câu lệnh SQL
            command.CommandText = "insert into ACCOUNT\nvalues('" + account.userName + "', '" + hashPass + "');";

            //Thực thi
            try {
                command.ExecuteNonQuery();

                connection.Close();
                return View("Create");
            }catch {
                TempData["ErrorMessage"] = "*Tên tài khoản đã tồn tại! Xin hãy thử tên khác!";
                TempData["Password"] = account.password;
                TempData["Username"]=account.userName;
                connection.Close();
                return View("Register");
            }
        }

        //Tìm tài khoản
        [HttpPost]
        public IActionResult SearchAcc(string keyword)
        {
            if (keyword == null || keyword == "") return View("~/Views/Home/Search.cshtml");

            connectionString();
            connection.Open(); 
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "SELECT USERNAME FROM ACCOUNT WHERE USERNAME LIKE '%" + keyword+"%';";

            //Thực thi
            dataReader = command.ExecuteReader();

            //Lấy kết quả
            List<string> accList = new List<string>();
            int index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    if(_contextAccessor.HttpContext.Session.GetString("username")!=null)
                        if (dataReader.GetString(0) == _contextAccessor.HttpContext.Session.GetString("username"))
                            continue;
                    accList.Add(dataReader.GetString(0));
                    index++;
                }
            }

            //Gửi tới giao diện
            TempData["keyword"]=keyword;
            TempData["by"] = "Acc";
            ISearch list = new StrListSearch(accList);
            return View("~/Views/Home/Search.cshtml",list);
        }

        //Trang cá nhân
        [HttpGet, HttpPost]
        public IActionResult Profile(string? username)
        {
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Trang của mình hay người khác
            if (username != null)
                TempData["username"] = username;
            else
                TempData["username"] = _contextAccessor.HttpContext.Session.GetString("username");


            //Câu lệnh SQL
            command.CommandText = "SELECT * FROM PICTURE WHERE OWNER = '" + TempData["username"] + "';";

            //Thực thi
            dataReader = command.ExecuteReader();

            //Lấy kết quả
            List<Picture> picList = new List<Picture>();
            int index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    picList.Add(new Picture(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                    index++;
                }
            }
            dataReader.Close();

            //Câu lệnh SQL
            command.CommandText = "SELECT * FROM FOLLOW WHERE FOLLOWER = '" + 
                _contextAccessor.HttpContext.Session.GetString("username") + "' and BE_FOLLOW = '" + TempData["username"] +"';";

            //Thực thi
            dataReader = command.ExecuteReader();

            if (dataReader.HasRows) { TempData["followed"] = 1; }

            return View(picList);
        }

        //Giao diện quan tâm
        public IActionResult Follow()
        {
            connectionString();
            connection.Open();
            command.Connection = connection;

            //-Lấy danh sách tài khoản
            //Câu lệnh SQL
            command.CommandText = "SELECT BE_FOLLOW FROM FOLLOW WHERE FOLLOWER = '" + 
                _contextAccessor.HttpContext.Session.GetString("username") + "';";

            //Thực thi
            dataReader = command.ExecuteReader();

            //Lấy kết quả
            List<string> accList = new List<string>();
            int index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    if (_contextAccessor.HttpContext.Session.GetString("username") != null)
                        if (dataReader.GetString(0) == _contextAccessor.HttpContext.Session.GetString("username"))
                            continue;
                    accList.Add(dataReader.GetString(0));
                    index++;
                }
            }
            dataReader.Close();


            //-Lấy danh sách ảnh
            //Câu lệnh SQL
            command.CommandText = "SELECT PICTURE.* FROM SAVE_PIC " +
                "JOIN PICTURE ON SAVE_PIC.BE_SAVE = PICTURE.ID " +
                "WHERE SAVE_PIC.SAVER = '" + _contextAccessor.HttpContext.Session.GetString("username") + "';";

            //Thực thi
            dataReader = command.ExecuteReader();

            //Lấy kết quả
            List<Picture> picList = new List<Picture>();
            index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    picList.Add(new Picture(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                    index++;
                }
            }

            //Gửi tới giao diện
            List<ICare> Lists = new List<ICare>();
            Lists.Add( new UserListCare(accList));
            Lists.Add(new PicListCare(picList));

            return View(Lists);
        }

        //Thêm Follow
        [HttpPost, HttpGet]
        public IActionResult AddFollow(string username)
        {
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "insert into FOLLOW values('" + 
                _contextAccessor.HttpContext.Session.GetString("username") + "', '" + username + "');";

            //Thực thi
            try
            {
                command.ExecuteNonQuery();

                connection.Close();
                return Redirect("/profile/"+username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TempData["ErrorMessage"] = "*Đã có lỗi xảy ra! Thêm quan tâm thất bại!";
                connection.Close();
                return View();
            }
        }

        //Xóa Follow
        [HttpPost, HttpGet]
        public IActionResult DeleteFollow(string username)
        {
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "delete from FOLLOW where FOLLOWER = '" +
                _contextAccessor.HttpContext.Session.GetString("username") + "' and BE_FOLLOW = '" + username + "';";

            //Thực thi
            try
            {
                command.ExecuteNonQuery();

                connection.Close();
                return Redirect("/profile/" + username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TempData["ErrorMessage"] = "*Đã có lỗi xảy ra! Xóa quan tâm thất bại!";
                connection.Close();
                return View("AddFollow");
            }
        }

        void connectionString()
        {
            connection.ConnectionString = "Server=tcp:nt205-hk5.database.windows.net,1433;Initial Catalog=SUrPic;Persist Security Info=False;" +
                "User ID=nt205hk5;Password=Huyisnoobzgamer0@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }
    }
}
