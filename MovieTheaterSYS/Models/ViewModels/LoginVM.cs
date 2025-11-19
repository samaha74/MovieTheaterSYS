namespace MovieTheaterSYS.Models.ViewModels
{
    public class LoginVM
    {
        public int id { get; set; }
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
