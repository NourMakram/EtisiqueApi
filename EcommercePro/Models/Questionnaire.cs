using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public int RequestCode { get; set; }
        public int RequestId { set; get; }

        [ForeignKey("Customer")]
        public string? CustomerId { set; get; }
        public ApplicationUser? Customer { set; get; }
        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة

        [ForeignKey("Technician")]
        public string TechnicianId { get; set; }
        public ApplicationUser? Technician { set; get; }

        public string? TechnicianNote { get; set; }

        public string? ServiceNote { get; set; }
        public string? ManagerNote { get; set; }

        public int Rating { get; set; }

        public DateOnly CreatedDate { set; get; }

        public List<TechnicianQusetions>? TechnicianQusetions { set; get; }



    }
}