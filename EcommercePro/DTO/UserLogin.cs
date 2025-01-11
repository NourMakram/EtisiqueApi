using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
    public class UserLogin
    {

        [Required(ErrorMessage ="Enter The Phone Number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="Enter The Password")]
        public string Password { get; set; }
    }
}
