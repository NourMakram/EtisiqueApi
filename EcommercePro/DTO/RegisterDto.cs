using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class RegisterDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter The Full Name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Enter Your Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Your Phone Number")]
        [RegularExpression("^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$", ErrorMessage = "Enter phone number in form of 05xxxxxxxx")]
        [Unique]
        public string PhoneNumber { get; set; }
        public string? Password { get; set; }
        [Required(ErrorMessage = "Enter the projectId")]
        public int projectId { get; set; }
        [Required(ErrorMessage = "TypeProject is Required")]
        public string TypeProject { get; set; } //مالك - مستأجر

        [Required(ErrorMessage = "City is Required")]
        public string City { set; get; }

        [Required(ErrorMessage = "Building is Required")]
        public string BulidingName { get; set; }

        [Required(ErrorMessage = "UnitNo is Required")]
        public int UnitNo { get; set; }

        [Required(ErrorMessage = "Latitude value is Required")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Longitude value is Required")]
        public double Longitude { get; set; }
        public string? Image { set; get; }
        public IFormFile? formFile { set; get; }
    }
}
