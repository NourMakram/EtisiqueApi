using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class CommonPartsResponse
    {
        public int id { set; get; }
        public int RequestCode { set; get; }

         public string projectName { get; set; }
        public string Type { set; get; }
 
        public string BuildingName { set; get; }
		public int? UnitNo { set; get; }

		public string? Position { set; get; } //الموقع

         public string ServiceType { set; get; }
 
        public List<string>? SubServices { set; get; }

        public string IsUrgent { set; get; }

        public string Description { set; get; }

        public string RequestStuatus { set; get; }

        public string CreatedDate { set; get; }

        public string UpdatedDate { set; get; }

        public string DateOfVisit { set; get; }

        public int NumDays { set; get; } = 0;

        public string RequestImage { set; get; }
        
        public string? TechnicianName { set; get; }
        public string? TechnicianPhone { set; get; }

        public string? ManagerName { set; get; }
        public string? ManagerPhone { set; get; }
        public string ManagerNote { set; get; }
        public List<string> images { set; get; }
		public ServicesVerificationsDto servicesVerification { set; get; }
        public int ProjectId { set; get; }

        public string timeElasped { set; get; }


    }
}
