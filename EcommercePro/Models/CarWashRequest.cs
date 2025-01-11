using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EtisiqueApi.Models
{
    public class CarWashRequest
    {
        public int id { set; get; }
        public int RequestCode { set; get; }

        [ForeignKey("Project")]
        public int projectId { get; set; }
        public Project? Project { get; set; }
        public string ClientName { set; get; }
        public string ClientPhone { set; get; }
        public string Address { set; get; }

        [ForeignKey("Location")]
        public int RequestLocationId { set; get; }
        public Location? Location { set; get; }

        public string? Description { set; get; }

        public string RequestStuatus { set; get; }

        public DateTime? CreatedDate { set; get; }
        public DateTime? TransferDate { set; get; }
        public DateTime? EndDate { set; get; }

        public string Period { set; get; }

        public DateOnly DateOfVisit { set; get; }

        public int NumDays { set; get; }

        public string? CarImage { set; get; }

        [ForeignKey("CarWashContract")]
        public int ContractId { set; get; }
        public CarWashContract? CarWashContract { set; get; }
        public string PaymentImage { set; get; }
        public string  CarNumberLetters { set; get; }
        public string CarNumberDigits { set; get; }

        [ForeignKey("applicationUser")]
        public string? TechnicianId { set; get; }
        public ApplicationUser? applicationUser { set; get; }

        [ForeignKey("Customer")]
        public int CustomerId { set; get; }
        public Customer? Customer { set; get; }
    }
}
