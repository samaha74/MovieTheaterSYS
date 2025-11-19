using System.ComponentModel.DataAnnotations;

namespace MovieTheaterSYS.Models.ViewModels
{
    public class ResetPasswordVM
    {
        public int id { get; set; }

        [DataType(DataType.Password)]
        public string Password {  get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
