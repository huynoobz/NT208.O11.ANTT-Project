namespace SUrPic.Models
{
    public class Picture
    {
        public IFormFile File {get;set;}
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; }
        public List<string> Tags { get; set; }
        public Picture() { }
        public Picture(int id, string name, string owner, string path)
        {
            Id = id;
            Name = name;
            Owner = owner;
            Path = path;
        }
        public Picture(IFormFile file, int id, string name, string owner, string path, List<string> tags)
        {
            File = file;
            Id = id;
            Name = name;
            Owner = owner;
            Path = path;
            Tags = tags;
        }
    }
}
