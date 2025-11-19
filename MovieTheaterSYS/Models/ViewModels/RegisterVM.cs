using System.ComponentModel.DataAnnotations;

namespace MovieTheaterSYS.Models.ViewModels
{
    public class RegisterVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    
        public string UserName { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

    }
}
