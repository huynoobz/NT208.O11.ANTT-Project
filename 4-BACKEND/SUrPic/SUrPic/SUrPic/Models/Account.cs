namespace SUrPic.Models
{
    public class Account
    {
        public string userName {  get; set; }
        public string password { get; set; }

        public string toString()
        {
            return "username " + userName + "\npass " + password;
        }
    }
}
