using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class ApartmentServicesRequest
    {
        public int id { set; get; }
        public int RequestCode { set; get; }
       
        [ForeignKey("Project")]
        public int projectId { get; set; }
        public Project? Project { get; set; }

        [ForeignKey("ServicesType")]
        public int ServiceTypeId { set; get; }
        public ApartmentService? ServiceType { set; get; }

        public string? BuildingName { set; get; }
        public int? UnitNo { set; get; }

        public List<ApartmentSubServicesRequest>? SubServices { set; get; }

        public string ClientName { set; get; }
        public string ClientPhone { set; get; }
        public string Address { set; get; }

        [ForeignKey("Location")]
        public int? RequestLocationId { set; get; }
        public Location? Location { set; get; }

        public string? Description { set; get; }

        public string RequestStuatus { set; get; }

        public DateTime CreatedDate { set; get; }
 
        public string Period { set; get; }

        public DateTime DateOfVisit { set; get; }
        public DateTime UpdatedDate { set; get; } = DateTime.UtcNow;

		public string? RequestImage { set; get; }

       // public int NumDays { set; get; } = 0;
        public string? TimeElapsed { set; get; }
        public List<RequestsImages>? images { set; get; }

        public bool? hasGuarantee { set; get; }

        [ForeignKey("ApplicationUser")]
        public string? TechnicianId { set; get; }
        public ApplicationUser? ApplicationUser { set; get; }

        [ForeignKey("Customer")]
        public int? CustomerId { set; get; }
        public Customer? Customer { set; get; }
        public string? CloseCode {  set; get; }
        public ApartmentServicesVerifications? ApartmentServicesVerifications { set; get; }

        public bool IsConfirmation { get; set; } = false;





    }
}
