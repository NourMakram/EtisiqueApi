using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class RequestsCommonParts
    {
        public int id { set; get; }
        public int Type { set; get; } //داخل =1 او خارج =2 
        public int RequestCode { set; get; }

        [ForeignKey("Project")]
        public int projectId { get; set; }
        public Project? Project { get; set; }

        public string BuildingName { set; get; }
		
        public int? UnitNo { set; get; }
		public string? Position { set; get; } //الموقع

        [ForeignKey("ServicesType")]
        public int ServiceTypeId { set; get; }
        public ApartmentService? ServiceType { set; get; }

        public List<ApartmentSubServicesRequest>? SubServices { set; get; }

        public bool IsUrgent { set; get; } 

        public string Description { set; get; }

        public string RequestStuatus { set; get; }

        public DateTime CreatedDate { set; get; }

        public DateTime UpdatedDate { set; get; } 

        public DateTime DateOfVisit { set; get; }

         public string? TimeElapsed { set; get; }
        public List<RequestsImages>? images { set; get; }    

        public string? RequestImage { set; get; }

        [ForeignKey("Technician")]
        public string? TechnicianId { set; get; }
        public ApplicationUser? Technician { set; get; }

        [ForeignKey("Manager")]
        public string? ManagerId { set; get; }
        public ApplicationUser? Manager { set; get; }

        public string? ManagerNote { set; get; }

        public CommonPartsVerifications? CommonPartsVerifications { set; get; }

		public bool IsConfirmation { get; set; } = false;

        public bool IsCleaning { get; set; } = false;


	}
}
