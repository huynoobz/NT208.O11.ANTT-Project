namespace SUrPic.Models
{
    public interface ICare
    {
    }
    public class UserListCare : ICare
    {
        public List<string> List { get; set; }
        public UserListCare(List<string> strings) {List = strings; }
    }
    public class PicListCare : ICare
    {
        public List<Picture> List { get; set; }
        public PicListCare(List<Picture> pictures) { List = pictures; }
    }

}
