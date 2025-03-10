using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class UserDto
    {
    public  string Id {  get; set; }
    [Required(ErrorMessage ="Enter The Full Name")]
     public string FullName { get; set; }
    [Required(ErrorMessage ="Enter Your Email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Enter Your Phone Number")]
    [RegularExpression("^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$", ErrorMessage = "Enter phone number in form of 05xxxxxxxx")]
    [Unique]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage ="Enter the Password")]
    public string Password { get; set; }
   [Required(ErrorMessage = "Enter the Job")]
    public List<string> Job { get; set; }
    public int? projectId { get; set; }
        public string? TypeProject { get; set; } //مالك - مستأجر

         public string? City { set; get; }

         public string? BulidingName { get; set; }

         public int? UnitNo { get; set; }

         public double? Latitude { get; set; }
         public double? Longitude { get; set; }
        public string? Image { set; get; }
       public IFormFile? formFile { set; get; }
        public List<int>? Projects { set; get; }


    }
    public class updateDto
    {
        [Required(ErrorMessage = "userid is Required")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Enter The Full Name")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Enter Your Email")]
        [UniqueEmail]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Your Phone Number")]
        [RegularExpression("^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$", ErrorMessage = "Enter phone number in form of 05xxxxxxxx")]
        [Unique]
        public string PhoneNumber { get; set; }
        public string? Password { get; set; }
        [Required(ErrorMessage = "Enter the Job")]
        public List<string> Job { get; set; }
        public int? projectId { get; set; }

        public string? Image { set; get; }
        public IFormFile? formFile { set; get; }
        public List<int>? Projects { set; get; }


    }
    public class UserBasicInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class userData
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Role { get; set; }
    }
    public class updateData
    {
        [Required(ErrorMessage ="UserId is Required")]
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? formFile { set; get; }


    }
    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string phoneNumber { get; set; }
        public string Email { set; get; }
        public string BuildingName { get; set; }
        public int UnitNum { get; set; }
        public string project { set; get; }
        public string userId { set; get; }
        public string ReceivedCode { set; get; }
        public string ReceivedDate { set; get; }
        public DateOnly? GuaranteeStart { set; get; }
        public DateOnly? GuaranteeEnd { set; get; }


    }
    public class unit
    {
		public int Id { get; set; }
 		public string BuildingName { get; set; }
		public int UnitNum { get; set; }
		public string project { set; get; }
	}
    public class UnitDetails
    {
        public int id { set; get; }
         public string FullName { set; get; }
        public string Email { set; get; }
        public string PhoneNumber { set; get; }
        public int projectId { set; get; }
        public string TypeProject { set; get; }
        public string City { set; get; }
        public int UnitNo { set; get; }
        public double Latitude { set; get; }
        public double Longitude { set; get; }
        public string UserId { set; get; }
        public string BuidingName { set; get; }
        public DateOnly? GuaranteeStart { set; get; }
        public DateOnly? GuaranteeEnd { set; get; }
    }
    public class UnitsManagmentDto
    {
        [Required]
        public int id { set; get; }
        [Required]
        public DateOnly start { set; get; }
        [Required]
        public DateOnly end { set; get; }

    }
    public class NewUnit
    {
        public int id { set; get; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int UnitNo { set; get; }
        [Required]
        public string BuidingName { set; get; }
        [Required]
        public int projectId { set; get; }
        [Required]
        public string TypeProject { set; get; }

        public DateOnly? GuaranteeStart { set; get; }
        public DateOnly? GuaranteeEnd { set; get; }


    }
}
