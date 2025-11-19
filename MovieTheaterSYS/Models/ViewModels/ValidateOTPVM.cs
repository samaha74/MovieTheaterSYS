namespace MovieTheaterSYS.Models.ViewModels
{
    public class ValidateOTPVM
    {
        public int id {  get; set; }
        public string UserId { get; set; }
        public ApplicationUser user { get; set; }
        public string OTP { get; set; }
    }
}
