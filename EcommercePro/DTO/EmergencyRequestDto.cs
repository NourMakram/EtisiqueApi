using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class EmergencyRequestDto
    {
        public class EmergencyRequestToAdd
        {
            public int id { set; get; }
            [Required(ErrorMessage ="Enter the ProjectId")]
            public int projectId { get; set; }
            [Required(ErrorMessage = "Enter the EmergencyId")]
            public int EmergencyId { set; get; }
            public string? BuildingName { set; get; }
            public int UnitNo { set; get; }
            public string? Description { set; get; }

            [Required(ErrorMessage = "Enter the CustomerId")]
            public int CustomerId { set; get; }
            public List<IFormFile>? formFiles { get; set; }
            public List<string>? images { set; get; }
            public string ClientName { set; get; }
            public string ClientPhone { set; get; }
            public string Email { set; get; }
            public string Address { set; get; }
            public double Latitude { set; get; }
            public double Longitude { set; get; }
        }
        public class EmergencyRequestToShow
        {
            public int id { set; get; }
            public int RequestCode { set; get; }
            public string projectName { get; set; }
            public string BulidingName { get; set; }
            public string UnitNo { get; set; }
            public string EmergencyType { set; get; }
             public string ClientName { set; get; }
            public string ClientPhone { set; get; }
            public string Address { set; get; }
            public string? Description { set; get; }
            public string RequestStuatus { set; get; }
            public string CreatedDate { set; get; }
            public string TimeElapsed { set; get; }
            public List<string>? images { set; get; }
            public string? TechnicianName { set; get; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string CloseCode { set; get; }
            public string? TechnicianPhone { set; get; }

        }
    }
}
