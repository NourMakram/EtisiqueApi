using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class CommonPartsDto
    {
        public int id { set; get; }

		[Required(ErrorMessage = "Type is Required")]
		public int Type { set; get; } //داخل =1 او خارج =2  

		 [Required(ErrorMessage = "Project is Required")]
         public int projectId { get; set; }

        [Required(ErrorMessage = "BuildingName is Required")]
        public string BuildingName { set; get; }

		public int? UnitNo { set; get; }
        public string? Position { set; get; } //الموقع


        [Required(ErrorMessage = "ServiceTypeId is Required")]
        public int ServiceTypeId { set; get; }
 
        public List<int>? SubServices { set; get; }

        public bool IsUrgent { set; get; } = false;

        //[ForeignKey("Location")]
        //public int RequestLocationId { set; get; }
        //public Location? Location { set; get; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { set; get; }
        [Required(ErrorMessage = "DateOfVisit is Required")]
        public DateTime DateOfVisit { set; get; }

        [Required(ErrorMessage = "RequestImage is Required")]
        public List<IFormFile>? formFiles { get; set; }

         [Required(ErrorMessage = "ManagerId is Required")]
        public string? ManagerId { set; get; }
 
     }
}
