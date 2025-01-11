using EcommercePro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class KitchenServiceResponse
    {
        public int id { set; get; }
        public int RequestCode { set; get; }
         public string projectName { get; set; }
         public string BuildingName { set; get; }
        public int UnitNo { set; get; }
        public string ClientName { set; get; }
        public string ClientPhone { set; get; }
 public string TimeElapsed { set; get; }
        public string Area { set; get; } // 15 * 12

        public double Latitude { get; set; }
         public double Longitude { get; set; }

        public string? Description { set; get; }

        public string RequestStuatus { set; get; }

        public string CreatedDate { set; get; }

        public string Period { set; get; }

        public int NumDays { set; get; } = 0;

        public string? RequestImage { set; get; }

        public string? AgreementFile { set; get; }

        public string? TechnicianName { set; get; }
        public string? TechnicianPhone { set; get; }

        public string closeCode { set; get; }

        public List<string> images { set; get; }

    }
}
