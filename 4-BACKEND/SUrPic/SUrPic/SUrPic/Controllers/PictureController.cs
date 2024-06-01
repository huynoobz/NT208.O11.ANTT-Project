using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using SUrPic.Models;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Principal;

namespace SUrPic.Controllers
{
    public class PictureController : Controller
    {
        //Session
        private readonly IHttpContextAccessor _contextAccessor;

        SqlConnection connection = new SqlConnection();
        SqlCommand command = new SqlCommand();
        SqlDataReader? dataReader;

        public PictureController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Tìm hình ảnh
        [HttpPost]
        public IActionResult SearchPic(string keyword)
        {
            if(keyword==null || keyword=="" || keyword.Split().Length==0) return View("~/Views/Home/Search.cshtml");

            connectionString();
            connection.Open();
            command.Connection = connection;

            Console.WriteLine(keyword);
            //Câu lệnh SQL
            if(keyword.Split().Length==1)
                command.CommandText = "SELECT * FROM PICTURE WHERE NAME LIKE '%" + keyword + "%' OR TAGS LIKE '%"+keyword+"%';";
            else
            {
                command.CommandText = "SELECT * FROM PICTURE WHERE NAME LIKE '%" + keyword + "%' OR (";
                foreach(string tag in keyword.Split())
                {
                    command.CommandText += "TAGS LIKE '%"+tag+"%' AND ";
                }
                command.CommandText = command.CommandText.Substring(0, command.CommandText.Length - 5) + ");";
            }

            //Thực thi
            dataReader = command.ExecuteReader();

            //Lấy kết quả
            List<Picture> pictureList = new List<Picture>();
            int index = 0;
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    pictureList.Add(new Picture(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3)));
                    index++;
                }
            }

            //Gửi tới giao diện
            TempData["keyword"] = keyword;
            TempData["by"] = "Img";
            ISearch list = new PicListSearch(pictureList);
            return View("~/Views/Home/Search.cshtml", list);
        }

        //Giao diện tải ảnh lên
        public IActionResult Upload()
        {
            return View();
        }

        //Tải ảnh lên
        [HttpPost]
        public async Task<IActionResult> UploadPic(Picture picture)
        {
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            command.CommandText = "select COUNT(*) from PICTURE;";
            dataReader = command.ExecuteReader();
            dataReader.Read();

            //Tạo đường lưu file
            picture.Path="~/images/"+dataReader.GetInt32(0).ToString()+picture.File.FileName.Substring(picture.File.FileName.Length-4);
            dataReader.Close();
            string savePath = Path.GetFullPath("wwwroot/" + picture.Path.Substring(1));

            string strTags ="";
            foreach(string tag in picture.Tags)
            {
                strTags += tag+" ";
            }

            picture.Owner = _contextAccessor.HttpContext.Session.GetString("username");

            //Câu lệnh SQL
            command.CommandText = "insert into PICTURE (NAME, OWNER, PATH, TAGS) values('" + picture.Name+"', '"+picture.Owner+"', '"+picture.Path+"', '"+strTags+"');";

            //Thực thi
            try
            {
                command.ExecuteNonQuery();

                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    await picture.File.CopyToAsync(stream);
                }

                connection.Close();
                return View();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                TempData["ErrorMessage"] = "*Đã xảy ra lỗi, ảnh tải lên không thành công!";
                connection.Close();
                return View();
            }
        }

        //Thêm lưu ảnh
        [HttpPost]
        public JsonResult AddSave(string id)
        {
            Console.WriteLine(id);
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "insert into SAVE_PIC (SAVER,BE_SAVE) values('" +
                _contextAccessor.HttpContext.Session.GetString("username") + "', '" + id + "');";

            //Thực thi
            try
            {
                command.ExecuteNonQuery();

                connection.Close();
                return Json(new { stat="ok" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                connection.Close();
                return Json(new { });
            }

        }

        //Kiểm tra lưu ảnh
        [HttpPost]
        public JsonResult CheckSave(string id)
        {
            Console.WriteLine(id);
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "select * from SAVE_PIC where SAVER = '"+
                _contextAccessor.HttpContext.Session.GetString("username")+"' and BE_SAVE = '"+id+"';";

            //Thực thi
            try
            {
                dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    connection.Close();
                    return Json(new { stat = "ok" });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            connection.Close();
            return Json(new { });
        }

        //Xóa lưu ảnh
        [HttpPost]
        public JsonResult DeleteSave(string id)
        {
            Console.WriteLine(id);
            //Kết nối cơ sở dữ liệu
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Câu lệnh SQL
            command.CommandText = "delete from SAVE_PIC where SAVER = '" +
                _contextAccessor.HttpContext.Session.GetString("username") + "' and BE_SAVE = '" + id + "';";

            //Thực thi
            try
            {
                command.ExecuteNonQuery();

                connection.Close();
                return Json(new { stat = "ok" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                connection.Close();
                return Json(new { });
            }
        }

        //Xóa hình ảnh
        [HttpPost]
        public IActionResult DeletePic(string id)
        {
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Xác thực chủ ảnh
            command.CommandText = "select * from PICTURE where OWNER = '" +
                _contextAccessor.HttpContext.Session.GetString("username") + "' and ID = '" + id + "';";
            dataReader=command.ExecuteReader();
            bool safe = dataReader.HasRows;
            dataReader.Close();

            if (safe)
            {
                //Câu lệnh SQL
                command.CommandText = "select PATH from PICTURE where ID = '" + id + "';";
                dataReader = command.ExecuteReader();
                dataReader.Read();
                
                //Xóa tệp
                System.IO.File.Delete(Path.GetFullPath("wwwroot/" + dataReader.GetString(0).Substring(1)));
                dataReader.Close();

                //Câu lệnh SQL
                command.CommandText = "delete from SAVE_PIC where BE_SAVE = '" + id + "';";

                //Thực thi
                try
                {
                    command.ExecuteNonQuery();

                    //Câu lệnh SQL
                    command.CommandText = "delete from PICTURE where ID = '" + id + "';";
                    command.ExecuteNonQuery();

                    connection.Close();
                    return RedirectToAction("Profile","Account");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            TempData["error"] = "*Đã xảy ra lỗi! Xóa ảnh thất bại!";
            connection.Close();
            return RedirectToAction("Profile", "Account");
        }

        //Xem ảnh
        [HttpPost, HttpGet]
        public IActionResult ViewPic(string id)
        {
            connectionString();
            connection.Open();
            command.Connection = connection;

            //Tìm ảnh
            command.CommandText = "select * from PICTURE where ID = '" + id + "';";
            dataReader = command.ExecuteReader();

            Picture picture = new Picture();
            if (dataReader.HasRows)
            {
                dataReader.Read();
                picture=new Picture(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3));
            }
            else
            {
                TempData["error"] = "*Đã xảy ra lỗi! Xem ảnh thất bại!";
            }
            connection.Close();
            return View(picture);
        }


        void connectionString()
        {
            connection.ConnectionString = "Server=tcp:nt205-hk5.database.windows.net,1433;Initial Catalog=SUrPic;" +
                "Persist Security Info=False;User ID=nt205hk5;Password=Huyisnoobzgamer0@;MultipleActiveResultSets=False;" +
                "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }


    }
}
