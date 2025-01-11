using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class ApartmentSubServicesRequest
    {
        public int Id { get; set; }

        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة

        //[ForeignKey("ApartmentServicesRequest")]
        public int RequestId { get; set; }
       // public ApartmentServicesRequest? ApartmentServicesRequest { get; set; }

        [ForeignKey("ApartmentServicesType")]
        public int ApServiceTypeId { get; set; }
        public ApartmentServicesType? ApartmentServicesType { get; set; }
    }
}
