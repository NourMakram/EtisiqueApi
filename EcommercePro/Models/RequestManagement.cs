using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class RequestManagement
    {
        public int Id { get; set; }
        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة
        public int RequestId { get; set; }

        public string ProcedureType { get; set; } //تاجيل - اغلاق - تحويل

        public string Notes { get; set; }
        public List<Attchments>? Attachments { set;get; }

        [ForeignKey("CreatedBy")]
        public string userId { set; get; }
        public ApplicationUser? CreatedBy { set; get; }

        public DateTime CreatedDate { set; get; }


    }
}
