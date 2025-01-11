using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class Emergency
    {
        public int id { set; get; }
        public int RequestCode { set; get; }

        [ForeignKey("Project")]
        public int projectId { get; set; }
        public Project? Project { get; set; }

        [ForeignKey("EmergencyTypes")]
        public int EmergencyId { set; get; }    
        public EmergencyTypes? EmergencyTypes { set; get; }
        public string? BuildingName { set; get; }
        public int? UnitNo { set; get; }
        public string? Description { set; get; }
        public string RequestStuatus { set; get; }
        public DateTime CreatedDate { set; get; }
      //  public int NumDays { set; get; } = 0;
        public string? TimeElapsed { set; get; }
 
        [ForeignKey("ApplicationUser")]
        public string? TechnicianId { set; get; }
        public ApplicationUser? ApplicationUser { set; get; }

        [ForeignKey("Customer")]
        public int? CustomerId { set; get; }
        public Customer? Customer { set; get; }
        public string? CloseCode { set; get; }
        public List<RequestsImages>? images { set; get; }
    }
}
