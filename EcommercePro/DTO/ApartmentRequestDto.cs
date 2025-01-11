using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class ApartmentRequestDto
    {
        public int id { set; get; }
        public int RequestCode { set; get; }
        public string projectName { get; set; }
        public string BulidingName { get; set; }
        public string UnitNo { get; set; }
        public string ServiceType { set; get; }
        public List<string>? SubServices { set; get; }
        public string ClientName { set; get; }
        public string ClientPhone { set; get; }
        public string Address { set; get; }
        public string? Description { set; get; }
        public string RequestStuatus { set; get; }
        public string CreatedDate { set; get; }
         public string EndDate { set; get; }
         public string Period { set; get; }
        public string DateOfVisit { set; get; }
        public string timeElasped { set; get; }
        public string? RequestImage { set; get; }
        public string? TechnicianName { set; get; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CloseCode { set; get; }
        public string? TechnicianPhone { set; get; }
        public List<string> images { set; get; }
        public ServicesVerificationsDto servicesVerification { set; get; }

    }
    public class ServicesVerificationsDto
    {
        public string ApproverName{ get; set; }
        public bool? IsApproved { get; set; }
        public decimal Fees { set; get; }
        public string? Note { get; set; }
        public string? File { set; get; }
        public string? TimeElapsed { set; get; }
    }

}
