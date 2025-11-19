namespace MovieTheaterSYS.Models
{
    public class UserOTP
    {
        public UserOTP(string oTP, string userId)
        {
            OTP = oTP;
            CreatedDate = DateTime.UtcNow;
            ExpireDate = DateTime.UtcNow.AddMinutes(10);
            isValid = true;
            UserId = userId;
        }

        public int Id { get; set; }
        public string OTP { get; set; }

        public DateTime ExpireDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool isValid { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
}
