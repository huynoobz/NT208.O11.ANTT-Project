using Microsoft.AspNetCore.Mvc;
using SUrPic.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security;

namespace SUrPic.Controllers
{
    public class HomeController : Controller
    {
        //Session
        private readonly IHttpContextAccessor _contextAccessor;

        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        SqlDataReader dataReader;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        //Trang chủ
        [HttpGet, HttpPost]
        public IActionResult Index()
        {
            //Kết nối CSDL
            //connection.ConnectionString = "Server=tcp:nt205-hk5.database.windows.net,1433;Initial Catalog=SUrPic;Persist Security Info=False;" +
            //  "User ID=nt205hk5;Password=Huyisnoobzgamer0@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            connection.ConnectionString = "Data Source=HUY-LT01\\SQLEXPRESS;Initial Catalog=SUrPic_DB;Integrated Security=True;Encrypt=False";
            connection.Open();
            command.Connection = connection;

            //Truy vấn
            command.CommandText = "select * from PICTURE";
            dataReader = command.ExecuteReader();

            List<Picture> pictureList = new List<Picture>();
            int index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    if (_contextAccessor.HttpContext.Session.GetString("username") != null)
                        if (dataReader.GetString(2) == _contextAccessor.HttpContext.Session.GetString("username"))
                            continue;
                    pictureList.Add(new Picture(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                    index++;
                }
            }
            if (index > 100)
                pictureList = GetRandomElements<Picture>(pictureList, 100);

            return View(pictureList);
        }

        //Giao diện tìm kiếm
        public IActionResult Search()
        {
            return View("Search");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        static List<T> GetRandomElements<T>(List<T> originalList, int count)
        {
            Random random = new Random();
            List<T> randomElements = new List<T>();

            List<T> shuffledList = originalList.OrderBy(x => random.Next()).ToList();

            randomElements = shuffledList.Take(count).ToList();

            return randomElements;
        }
    }
}