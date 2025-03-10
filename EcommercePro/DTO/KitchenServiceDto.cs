using EcommercePro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class KitchenServiceDto
    {
        public int id { set; get; }
        [Required(ErrorMessage = "CustomerId  is Required")]
        public int CustomerId { set; get; }
        [Required(ErrorMessage = "projectId  is Required")]
        public int projectId { set; get; }
        [Required(ErrorMessage = "BuildingName  is Required")]
        public string BuildingName { set; get; }
        [Required(ErrorMessage = "UnitNo  is Required")]
        public int UnitNo { set; get; }
        [Required(ErrorMessage = "Client Name is Required")]
        public string ClientName { set; get; }
        [Required(ErrorMessage = "Client Phone is Required")]
        public string ClientPhone { set; get; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invaild Email Address")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Client Address is Required")]
        public string Address { set; get; }

        [Required(ErrorMessage = "Latitude value is Required")]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Longitude value is Required")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Period is Required")]
        //[AllowedValues(["10am:2pm", "4pm:8pm"])]
        public string Period { set; get; }
        [Required(ErrorMessage = "Area is Required")]
        public string Area { set; get; } // 15 * 12
        public string? RequestImage { set; get; }
        public IFormFile? FileUrl { set; get; }
        public string? Description { set; get; }

        public List<IFormFile>? formFiles { get; set; }



    }
}
