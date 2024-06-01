namespace SUrPic.Models
{
    public interface ISearch
    {

    }
    public class StrListSearch : ISearch
    {
        public StrListSearch(List<string> l) { StrList = l; }
        public List<string> StrList { get; set; }
    }
    public class PicListSearch : ISearch
    {
        public PicListSearch(List<Picture> pictures) { PicList = pictures; }
        public List<Picture> PicList { get; set; }
    }
}
