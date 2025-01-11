using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class KitchenServices
    {
        public int id { set; get; }
        public int RequestCode { set; get; }
        [ForeignKey("Project")]
        public int projectId { get; set; }
        public Project? Project { get; set; }
        public string  BuildingName { set; get; }
        public int  UnitNo { set; get; }
        public string ClientName { set; get; }
        public string ClientPhone { set; get; }
        public string Address { set; get; }

        public string Area { set; get; } // 15 * 12

        [ForeignKey("Location")]
        public int? RequestLocationId { set; get; }
        public Location? Location { set; get; }

        public string? Description { set; get; }

        public string RequestStuatus { set; get; }

        public DateTime CreatedDate { set; get; }

        public string Period { set; get; }

      //  public int NumDays { set; get; } = 0;

        public List<RequestsImages>? images { set; get; }

        public string? TimeElapsed { set; get; }

        public string? AgreementFile { set; get; }
        public string? CloseCode { get; set; }


        [ForeignKey("ApplicationUser")]
        public string? TechnicianId { set; get; }
        public ApplicationUser? Technician { set; get; }

        [ForeignKey("Customer")]
        public int? CustomerId { set; get; }
        public Customer? Customer { set; get; }

     }
}
