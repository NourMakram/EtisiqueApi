using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage ="UserId is Required")]
        public string userId { get; set; }
        [Required(ErrorMessage = "CurrentPassword is Required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "NewPassword is Required")]
        public string NewPassword { get; set; }
    }
    public class ResetPasswordDto
    {
        [Required(ErrorMessage ="Enter The Email Or Phone  Number Please")]
        public string EmailOrPhone { get; set; }
        [Required(ErrorMessage = "Enter The Password Please")]
        public string NewPassword { get; set; }

    }
}
